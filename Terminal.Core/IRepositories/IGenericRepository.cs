namespace Terminal.Core.IRepositories;

/// <summary>
/// Репозиторий для общих операций с базами данных.
/// </summary>
public interface IGenericRepository
{
    /// <summary>
    /// Получить множество строк из указанной таблицы.
    /// </summary>
    /// <param name="orderBy">Метод сортировки.</param>
    /// <param name="keyField">Имя поля являющегося ключом.</param>
    /// <param name="dbName">Имя класса контекста базы данных.</param>
    /// <param name="lastKey">Последний ключ для пагинации.</param>
    /// <param name="pageSize">Кол-во отдаваемых строк.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <typeparam name="T">Тип сущности.</typeparam>
    /// <returns>Список строк из таблицы.</returns>
    public Task<List<T>> GetALotOfStringFromArbitraryTableAsync<T>(
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
        string keyField,
        string dbName,
        int? lastKey = null,
        int pageSize = 300,
        CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Выполнить SQL скрипт.
    /// </summary>
    /// <param name="script">Скрипт.</param>
    /// <param name="dbName">Название контекста БД.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    /// <returns>Сообщение результата выполнения.</returns>
    public Task<int> ExecuteSqlAsync(string script, string dbName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить путь до файла главной БД.
    /// </summary>
    /// <returns>Глобальный путь.</returns>
    public string GetMainDbPath();
}