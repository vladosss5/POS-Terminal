using System.Runtime.InteropServices;

namespace Terminal.Core.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
public struct FileHeader
{
    /// <summary>
    /// Размер блока данных для передачи (обычно 5 * 1024 = 5120 байт)
    /// </summary>
    public uint BlockSize;
    
    /// <summary>
    /// Общий размер файла в байтах
    /// </summary>
    public uint FileSize;
    
    /// <summary>
    /// Тип файла
    /// </summary>
    public short FileType;
    
    /// <summary>
    /// Зарезервированное поле (не используется)
    /// </summary>
    public short Reserved;
    
    /// <summary>
    /// Имя файла (максимум 20 символов, null-terminated)
    /// </summary>
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string FileName;
    
    /// <summary>
    /// Создание заголовка файла
    /// </summary>
    /// <param name="blockSize">Размер блока</param>
    /// <param name="fileSize">Размер файла</param>
    /// <param name="fileType">Тип файла</param>
    /// <param name="fileName">Имя файла (обрежется до 20 символов)</param>
    public FileHeader(uint blockSize, uint fileSize, short fileType, string fileName)
    {
        BlockSize = blockSize;
        FileSize = fileSize;
        FileType = fileType;
        Reserved = 0;
        
        FileName = fileName.Length > 20 
            ? fileName[..20] 
            : fileName;
    }
    
    /// <summary>
    /// Создание заголовка файла с типом по умолчанию (0)
    /// </summary>
    public FileHeader(uint blockSize, uint fileSize, string fileName) 
        : this(blockSize, fileSize, 0, fileName)
    {
    }
    
    /// <summary>
    /// Копирующий конструктор
    /// </summary>
    public FileHeader(FileHeader other)
    {
        BlockSize = other.BlockSize;
        FileSize = other.FileSize;
        FileType = other.FileType;
        Reserved = other.Reserved;
        FileName = other.FileName;
    }
    
    /// <summary>
    /// Преобразование структуры в массив байт для отправки по сети
    /// </summary>
    public byte[] ToBytes()
    {
        var size = Marshal.SizeOf<FileHeader>();
        var result = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        
        try
        {
            Marshal.StructureToPtr(this, ptr, false);
            Marshal.Copy(ptr, result, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        
        return result;
    }
    
    /// <summary>
    /// Восстановление структуры из массива байт
    /// </summary>
    public static FileHeader FromBytes(byte[] bytes)
    {
        if (bytes == null || bytes.Length < Marshal.SizeOf<FileHeader>())
            throw new ArgumentException($"Invalid buffer size. Expected at least {Marshal.SizeOf<FileHeader>()} bytes");
        
        var ptr = Marshal.AllocHGlobal(bytes.Length);
        
        try
        {
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return Marshal.PtrToStructure<FileHeader>(ptr);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
    
    /// <summary>
    /// Асинхронное чтение заголовка из потока
    /// </summary>
    public static async Task<FileHeader> ReadFromStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var headerSize = Marshal.SizeOf<FileHeader>();
        var buffer = new byte[headerSize];
        
        var bytesRead = 0;
        while (bytesRead < headerSize)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(bytesRead, headerSize - bytesRead), cancellationToken);
            if (read == 0)
                throw new EndOfStreamException("Unexpected end of stream while reading FileHeader");
            bytesRead += read;
        }
        
        return FromBytes(buffer);
    }
    
    /// <summary>
    /// Запись заголовка в поток
    /// </summary>
    public async Task WriteToStreamAsync(Stream stream, CancellationToken cancellationToken = default)
    {
        var bytes = ToBytes();
        await stream.WriteAsync(bytes.AsMemory(0, bytes.Length), cancellationToken);
    }
    
    /// <summary>
    /// Проверка корректности заголовка
    /// </summary>
    public bool IsValid()
    {
        return BlockSize > 0 && 
               FileSize > 0 && 
               !string.IsNullOrEmpty(FileName) &&
               FileName.Length <= 20;
    }
    
    /// <summary>
    /// Получение количества блоков для передачи файла
    /// </summary>
    public uint GetBlockCount()
    {
        if (BlockSize == 0) return 0;
        return (uint)Math.Ceiling((double)FileSize / BlockSize);
    }
    
    /// <summary>
    /// Получение размера конкретного блока
    /// </summary>
    public uint GetBlockSize(uint blockIndex)
    {
        var blockCount = GetBlockCount();
        if (blockIndex >= blockCount) return 0;
        
        if (blockIndex == blockCount - 1)
        {
            // Последний блок может быть меньше
            var lastBlockSize = FileSize % BlockSize;
            return lastBlockSize == 0 ? BlockSize : lastBlockSize;
        }
        
        return BlockSize;
    }
    
    public override string ToString()
    {
        return $"FileHeader: Name='{FileName}', Size={FileSize}, BlockSize={BlockSize}, Type={FileType}";
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not FileHeader other)
            return false;
        
        return BlockSize == other.BlockSize &&
               FileSize == other.FileSize &&
               FileType == other.FileType &&
               Reserved == other.Reserved &&
               FileName == other.FileName;
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(BlockSize, FileSize, FileType, Reserved, FileName);
    }
}