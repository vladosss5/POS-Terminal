using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Terminal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOtherTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Temperature",
                table: "resource_code",
                type: "NUMERIC( 20, 4 )",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "NUMERIC(20,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ResourcePrice",
                table: "resource_code",
                type: "NUMERIC( 20, 3 )",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResourceName",
                table: "resource_code",
                type: "VARCHAR( 50 )",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ResourceKey",
                table: "resource_code",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<byte>(
                name: "IsShow",
                table: "resource_code",
                type: "TINYINT",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<double>(
                name: "Density",
                table: "resource_code",
                type: "NUMERIC( 20, 4 )",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "NUMERIC(20,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CollectionKey",
                table: "resource_code",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateTable(
                name: "allow",
                columns: table => new
                {
                    AllowKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    RequestServer = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_allow", x => x.AllowKey);
                });

            migrationBuilder.CreateTable(
                name: "bonus_change",
                columns: table => new
                {
                    BonusChangeShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    GraphicalNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    ElectronicNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    BonusChange = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShoppingCartKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerTerminalID = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardID = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CheckNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionDatetime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    SetOfGoodsGuid = table.Column<string>(type: "VARCHAR( 255 )", nullable: true),
                    CommodityGuid = table.Column<string>(type: "VARCHAR( 255 )", nullable: true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bonus_change", x => x.BonusChangeShopKey);
                });

            migrationBuilder.CreateTable(
                name: "card_password",
                columns: table => new
                {
                    CardPasswordKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GraphicalNumber = table.Column<long>(type: "NUMERIC( 20 )", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "NUMERIC( 20 )", nullable: true),
                    GraphicalNumberUPOS = table.Column<long>(type: "NUMERIC( 20 )", nullable: true),
                    Password = table.Column<int>(type: "INTEGER", nullable: true),
                    LastSessionStart = table.Column<long>(type: "BIGINT", nullable: true),
                    LastSessionEnd = table.Column<long>(type: "BIGINT", nullable: true),
                    SaleType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_card_password", x => x.CardPasswordKey);
                });

            migrationBuilder.CreateTable(
                name: "card_update",
                columns: table => new
                {
                    CardUpdateKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    TransactionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResultCode = table.Column<byte>(type: "TINYINT", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CorrectionType = table.Column<byte>(type: "TINYINT", nullable: true),
                    BeforeValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    AfterValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    UpdateDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    IsSent = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    ApplicationType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ParameterType = table.Column<byte>(type: "TINYINT", nullable: true),
                    EnterDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    StartDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    EndDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    ParameterRepValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    ParameterAddValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    ShiftTerminalKey = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_card_update", x => x.CardUpdateKey);
                });

            migrationBuilder.CreateTable(
                name: "corrections",
                columns: table => new
                {
                    CorrectionsKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    ApplicationType = table.Column<byte>(type: "TINYINT", nullable: true),
                    CorrectionType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ParameterType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ParameterRepValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    ParameterAddValue = table.Column<decimal>(type: "VARCHAR(255)", nullable: true),
                    EnterDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    StartDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    EndDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    IsDelete = table.Column<bool>(type: "BOOL", nullable: true),
                    Note = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_corrections", x => x.CorrectionsKey);
                });

            migrationBuilder.CreateTable(
                name: "dispenser",
                columns: table => new
                {
                    DispenserShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VendorKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    BeginBalance = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    EndBalance = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BeginBalanceCalculation = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    EndBalanceCalculation = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    Flags = table.Column<long>(type: "BIGINT", nullable: true),
                    VendorName = table.Column<string>(type: "VARCHAR(10)", nullable: true),
                    BeginTemperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    EndTemperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dispenser", x => x.DispenserShopKey);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    EventsKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    EventDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    EventType = table.Column<int>(type: "INTEGER", nullable: true),
                    EventObject = table.Column<int>(type: "INTEGER", nullable: true),
                    EventResult = table.Column<int>(type: "INTEGER", nullable: true),
                    EventInfo = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.EventsKey);
                });

            migrationBuilder.CreateTable(
                name: "issuer_fuel_table",
                columns: table => new
                {
                    IssuerFuelCodeKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IssuerID = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IsCard = table.Column<byte>(type: "TINYINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_issuer_fuel_table", x => x.IssuerFuelCodeKey);
                });

            migrationBuilder.CreateTable(
                name: "list_org",
                columns: table => new
                {
                    ListOrgKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationName = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_list_org", x => x.ListOrgKey);
                });

            migrationBuilder.CreateTable(
                name: "list_owner",
                columns: table => new
                {
                    ListOwnerKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerName = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    GraphicalNumber = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_list_owner", x => x.ListOwnerKey);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    PaymentShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PaymentSum = table.Column<decimal>(type: "NUMERIC(20,3)", nullable: true),
                    PaymentVolume = table.Column<decimal>(type: "NUMERIC(20,3)", nullable: true),
                    PaymentDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftTerminalKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    IsSent = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    GraphicalNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    AppValue = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    CommonApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    Guid = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    ShoppingCartKey = table.Column<int>(type: "INTEGER", nullable: true),
                    Flags = table.Column<int>(type: "INTEGER", nullable: true),
                    NZ = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    AppStatus = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.PaymentShopKey);
                });

            migrationBuilder.CreateTable(
                name: "pos_update",
                columns: table => new
                {
                    PosUpdateShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PosUpdateDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftTerminalKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    GraphicalNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    IsSent = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    Guid = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    BeforeValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    AfterValue = table.Column<string>(type: "VARCHAR(255)", nullable: true),
                    PosUpdateType = table.Column<byte>(type: "TINYINT", nullable: true),
                    AppStatus = table.Column<byte>(type: "TINYINT", nullable: true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    ChangeValue = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    CheckNumber = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pos_update", x => x.PosUpdateShopKey);
                });

            migrationBuilder.CreateTable(
                name: "prohibition",
                columns: table => new
                {
                    ProhibitionKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    Sign = table.Column<byte>(type: "TINYINT", nullable: true),
                    EndDate = table.Column<DateTime>(type: "DATETIME", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prohibition", x => x.ProhibitionKey);
                });

            migrationBuilder.CreateTable(
                name: "repayment",
                columns: table => new
                {
                    RepaymentShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RepaymentValue = table.Column<double>(type: "NUMERIC(20,3)", nullable: true),
                    RepaymentDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    RepaymentType = table.Column<byte>(type: "TINYINT", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftTerminalKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<long>(type: "NUMERIC( 14 )", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    IsSent = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    OwnerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    CardType = table.Column<byte>(type: "TINYINT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repayment", x => x.RepaymentShopKey);
                });

            migrationBuilder.CreateTable(
                name: "request",
                columns: table => new
                {
                    VendorKey = table.Column<int>(type: "INTEGER", nullable: false),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    InitialVolume = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    CompleteVolume = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShopCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    RequestType = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: true),
                    EndOfFilling = table.Column<int>(type: "INTEGER", nullable: true),
                    Flags = table.Column<int>(type: "INTEGER", nullable: true),
                    BaseType = table.Column<int>(type: "INTEGER", nullable: true),
                    DerivedType = table.Column<int>(type: "INTEGER", nullable: true),
                    LastVolume = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShoppingCartKey = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_request", x => x.VendorKey);
                });

            migrationBuilder.CreateTable(
                name: "selling",
                columns: table => new
                {
                    TransactionShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<long>(type: "NUMERIC( 14 )", nullable: true),
                    TransactionDatetime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    GraphicalNumber = table.Column<double>(type: "NUMERIC( 20 )", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "NUMERIC( 20 )", nullable: true),
                    BaseType = table.Column<int>(type: "INTEGER( 1 )", nullable: true),
                    DerivedType = table.Column<int>(type: "INTEGER( 1 )", nullable: true),
                    Amount = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShopCost = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShopBaseCost = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    SellingPrice = table.Column<int>(type: "NUMERIC( 10, 3 )", nullable: true),
                    ShoppingCartKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceName = table.Column<string>(type: "VARCHAR( 50 )", nullable: true),
                    IssuerTerminalID = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardID = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Sign = table.Column<string>(type: "VARCHAR( 255 )", nullable: true),
                    AppStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    AppMode = table.Column<int>(type: "INTEGER", nullable: true),
                    AppLimit = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppValue = table.Column<double>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppSecondLimit = table.Column<double>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppSecondValue = table.Column<double>(type: "NUMERIC( 20, 3 )", nullable: true),
                    CheckNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    ValidityPeriod = table.Column<long>(type: "INTEGER", nullable: true),
                    CommonApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    Guid = table.Column<Guid>(type: "VARCHAR(35)", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    BonusIn = table.Column<double>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BonusOut = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BonusInCost = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BonusOutCost = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    IsAccountRep = table.Column<int>(type: "INTEGER", nullable: true),
                    RequestedAmount = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    RequestedCost = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BasePrice = table.Column<int>(type: "NUMERIC( 10, 3 )", nullable: true),
                    ClientCost = table.Column<int>(type: "NUMERIC( 20, 3 )", nullable: true),
                    RequestFlags = table.Column<int>(type: "INTEGER", nullable: true, defaultValue: 0),
                    DelayedBonusType = table.Column<int>(type: "INTEGER", nullable: true),
                    ParcelPrice = table.Column<int>(type: "NUMERIC( 10, 3 )", nullable: true),
                    CommodityKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentOfCommodityKey = table.Column<int>(type: "INTEGER", nullable: true),
                    SetOfGoodsKey = table.Column<int>(type: "INTEGER", nullable: true),
                    VendorKey = table.Column<int>(type: "INTEGER", nullable: true),
                    Temperature = table.Column<int>(type: "NUMERIC( 10, 4 )", nullable: true),
                    Density = table.Column<int>(type: "NUMERIC( 10, 4 )", nullable: true),
                    CommodityGuid = table.Column<Guid>(type: "VARCHAR( 255 )", nullable: true),
                    SetOfGoodsGuid = table.Column<Guid>(type: "VARCHAR( 255 )", nullable: true),
                    BeginTemperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    EndTemperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    SellingFlags = table.Column<long>(type: "BIGINT", nullable: true),
                    Overflow = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    CardType = table.Column<int>(type: "INTEGER", nullable: true),
                    ExternalCode = table.Column<string>(type: "VARCHAR(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selling", x => x.TransactionShopKey);
                });

            migrationBuilder.CreateTable(
                name: "selling_ignore",
                columns: table => new
                {
                    TransactionShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    TransactionDatetime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    GraphicalNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    ElectronicNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    BaseType = table.Column<int>(type: "INTEGER( 1 )", nullable: true),
                    DerivedType = table.Column<int>(type: "INTEGER( 1 )", nullable: true),
                    Amount = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShopCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ShopBaseCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    SellingPrice = table.Column<decimal>(type: "NUMERIC( 10, 3 )", nullable: true),
                    ShoppingCartKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceName = table.Column<string>(type: "VARCHAR( 50 )", nullable: true),
                    IssuerTerminalID = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardID = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    TransactionCount = table.Column<int>(type: "INTEGER", nullable: true),
                    Sign = table.Column<string>(type: "VARCHAR( 255 )", nullable: true),
                    AppStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    AppMode = table.Column<int>(type: "INTEGER", nullable: true),
                    AppLimit = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppValue = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppSecondLimit = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppSecondValue = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    CheckNumber = table.Column<int>(type: "INTEGER", nullable: true),
                    ValidityPeriod = table.Column<int>(type: "INTEGER", nullable: true),
                    CommonApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    Guid = table.Column<string>(type: "VARCHAR(35)", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    BonusIn = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BonusOut = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BonusInCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BonusOutCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    IsAccountRep = table.Column<int>(type: "INTEGER", nullable: true),
                    RequestedAmount = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    RequestedCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    BasePrice = table.Column<decimal>(type: "NUMERIC( 10, 3 )", nullable: true),
                    ClientCost = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    RequestFlags = table.Column<int>(type: "INTEGER", nullable: true, defaultValue: 0),
                    DelayedBonusType = table.Column<int>(type: "INTEGER", nullable: true),
                    ParcelPrice = table.Column<decimal>(type: "NUMERIC( 10, 3 )", nullable: true),
                    CommodityKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PaymentOfCommodityKey = table.Column<int>(type: "INTEGER", nullable: true),
                    SetOfGoodsKey = table.Column<int>(type: "INTEGER", nullable: true),
                    VendorKey = table.Column<int>(type: "INTEGER", nullable: true),
                    Temperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    Density = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    CommodityGuid = table.Column<string>(type: "VARCHAR( 255 )", nullable: true),
                    SetOfGoodsGuid = table.Column<string>(type: "VARCHAR( 255 )", nullable: true),
                    BeginTemperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    EndTemperature = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    SellingFlags = table.Column<long>(type: "BIGINT", nullable: true),
                    Overflow = table.Column<decimal>(type: "NUMERIC( 10, 4 )", nullable: true),
                    CardType = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_selling_ignore", x => x.TransactionShopKey);
                });

            migrationBuilder.CreateTable(
                name: "sellingcoupon",
                columns: table => new
                {
                    SellingCouponShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<long>(type: "NUMERIC (14)", nullable: true),
                    TransactionDatetime = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    GraphicalNumber = table.Column<string>(type: "VARCHAR (250)", nullable: true),
                    ElectronicNumber = table.Column<long>(type: "BIGINT", nullable: true),
                    BaseType = table.Column<int>(type: "INTEGER (1)", nullable: true),
                    DerivedType = table.Column<int>(type: "INTEGER (1)", nullable: true),
                    UsedVolume = table.Column<int>(type: "NUMERIC (20, 3)", nullable: true),
                    ShoppingCartKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CouponType = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CommodityGuid = table.Column<Guid>(type: "VARCHAR (255)", nullable: true),
                    SetOfGoodsGuid = table.Column<Guid>(type: "VARCHAR (255)", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sellingcoupon", x => x.SellingCouponShopKey);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    SettingsKey = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.SettingsKey);
                });

            migrationBuilder.CreateTable(
                name: "shift",
                columns: table => new
                {
                    ShiftShopKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ShiftKey = table.Column<int>(type: "INTEGER", nullable: true),
                    TerminalKey = table.Column<long>(type: "NUMERIC( 14 )", nullable: true),
                    ErrorCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ShopKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ShiftDate = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    OperatorId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsOpened = table.Column<bool>(type: "BOOLEAN", nullable: true),
                    HttpSend = table.Column<int>(type: "INTEGER", nullable: true),
                    HttpRecv = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_shift", x => x.ShiftShopKey);
                });

            migrationBuilder.CreateTable(
                name: "transfer_card",
                columns: table => new
                {
                    TransferCardKey = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GraphicalNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    ElectronicNumber = table.Column<decimal>(type: "NUMERIC( 20 )", nullable: true),
                    AppStatus = table.Column<int>(type: "INTEGER", nullable: true),
                    AppMode = table.Column<int>(type: "INTEGER", nullable: true),
                    AppLimit = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppValue = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppSecondLimit = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    AppSecondValue = table.Column<decimal>(type: "NUMERIC( 20, 3 )", nullable: true),
                    ValidityPeriod = table.Column<long>(type: "BIGINT", nullable: true),
                    CommonApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    IssuerCardID = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationKey = table.Column<int>(type: "INTEGER", nullable: true),
                    PersonKey = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionKey = table.Column<int>(type: "INTEGER", nullable: true),
                    ResourceCode = table.Column<int>(type: "INTEGER", nullable: true),
                    ApplicationID = table.Column<int>(type: "INTEGER", nullable: true),
                    ParcelPrice = table.Column<decimal>(type: "NUMERIC( 10, 3 )", nullable: true),
                    OrganisationListOrgKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transfer_card", x => x.TransferCardKey);
                    table.ForeignKey(
                        name: "FK_transfer_card_list_org_OrganisationListOrgKey",
                        column: x => x.OrganisationListOrgKey,
                        principalTable: "list_org",
                        principalColumn: "ListOrgKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TerminalKey = table.Column<decimal>(type: "NUMERIC( 14 )", nullable: true),
                    Name = table.Column<string>(type: "VARCHAR( 50 )", nullable: true),
                    CardNumber = table.Column<int>(type: "NUMERIC( 16 )", nullable: true),
                    UserType = table.Column<int>(type: "NUMERIC( 2 )", nullable: true),
                    UserPassword = table.Column<string>(type: "VARCHAR( 35 )", nullable: true),
                    IssuerId = table.Column<int>(type: "INTEGER", nullable: true),
                    OrganisationId = table.Column<int>(type: "INTEGER", nullable: true),
                    ECardNumber = table.Column<decimal>(type: "NUMERIC( 16 )", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_users_list_org_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "list_org",
                        principalColumn: "ListOrgKey");
                });

            migrationBuilder.CreateIndex(
                name: "ResourceUnique",
                table: "resource_code",
                columns: new[] { "CollectionKey", "ResourceKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "BonChange1",
                table: "bonus_change",
                column: "SetOfGoodsGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "BalVender1",
                table: "dispenser",
                columns: new[] { "VendorKey", "ShiftKey", "TerminalKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "PaymentIndex",
                table: "payment",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "PosUpdateIndex",
                table: "pos_update",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "TrTer",
                table: "selling",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_transfer_card_OrganisationListOrgKey",
                table: "transfer_card",
                column: "OrganisationListOrgKey");

            migrationBuilder.CreateIndex(
                name: "IX_users_OrganisationId",
                table: "users",
                column: "OrganisationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "allow");

            migrationBuilder.DropTable(
                name: "bonus_change");

            migrationBuilder.DropTable(
                name: "card_password");

            migrationBuilder.DropTable(
                name: "card_update");

            migrationBuilder.DropTable(
                name: "corrections");

            migrationBuilder.DropTable(
                name: "dispenser");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "issuer_fuel_table");

            migrationBuilder.DropTable(
                name: "list_owner");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "pos_update");

            migrationBuilder.DropTable(
                name: "prohibition");

            migrationBuilder.DropTable(
                name: "repayment");

            migrationBuilder.DropTable(
                name: "request");

            migrationBuilder.DropTable(
                name: "selling");

            migrationBuilder.DropTable(
                name: "selling_ignore");

            migrationBuilder.DropTable(
                name: "sellingcoupon");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "shift");

            migrationBuilder.DropTable(
                name: "transfer_card");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "list_org");

            migrationBuilder.DropIndex(
                name: "ResourceUnique",
                table: "resource_code");

            migrationBuilder.AlterColumn<decimal>(
                name: "Temperature",
                table: "resource_code",
                type: "NUMERIC(20,4)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMERIC( 20, 4 )",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ResourcePrice",
                table: "resource_code",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "NUMERIC( 20, 3 )",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResourceName",
                table: "resource_code",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR( 50 )",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ResourceKey",
                table: "resource_code",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte>(
                name: "IsShow",
                table: "resource_code",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "TINYINT",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Density",
                table: "resource_code",
                type: "NUMERIC(20,4)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "NUMERIC( 20, 4 )",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CollectionKey",
                table: "resource_code",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }
    }
}
