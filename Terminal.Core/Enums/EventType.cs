// Terminal.Core/Enums/EventType.cs
namespace Terminal.Core.Enums;

/// <summary>
/// Типы событий, регистрируемых в системе
/// </summary>
public enum EventType
{
    /// <summary>
    /// Список запретов
    /// </summary>
    ProhibitionList = 0,
    
    /// <summary>
    /// Белый список (разрешения)
    /// </summary>
    AllowList = 1,
    
    /// <summary>
    /// Начало сеанса связи
    /// </summary>
    StartCommunication = 2,
    
    /// <summary>
    /// Авторизация терминала
    /// </summary>
    Authorization = 3,
    
    /// <summary>
    /// Упаковка данных для отправки
    /// </summary>
    PackData = 4,
    
    /// <summary>
    /// Отправка данных на терминал
    /// </summary>
    SendData = 5,
    
    /// <summary>
    /// Прием данных от терминала
    /// </summary>
    ReceiveData = 6,
    
    /// <summary>
    /// Завершение сеанса связи
    /// </summary>
    EndCommunication = 7,
    
    /// <summary>
    /// Обновление программного обеспечения
    /// </summary>
    UpdateSoftware = 8,
    
    /// <summary>
    /// Корректировки (изменения данных карт)
    /// </summary>
    Corrections = 9,
    
    /// <summary>
    /// Схема дискриминации (скидок/наценок)
    /// </summary>
    PosDsSchema = 10,
    
    /// <summary>
    /// Пользователи терминала
    /// </summary>
    Users = 11,
    
    /// <summary>
    /// Коды ресурсов (товаров/услуг)
    /// </summary>
    ResourceCode = 12,
    
    /// <summary>
    /// Таблица кодировок эмитентов
    /// </summary>
    IssuerFuelTable = 13,
    
    /// <summary>
    /// Цены на ресурсы
    /// </summary>
    Price = 14,
    
    /// <summary>
    /// Ограничения (лимиты)
    /// </summary>
    Limitation = 15,
    
    /// <summary>
    /// Ведомостные организации
    /// </summary>
    Organisation = 16,
    
    /// <summary>
    /// Ведомостные водители
    /// </summary>
    Owner = 17,
    
    /// <summary>
    /// Инкассация
    /// </summary>
    Incassation = 100
}