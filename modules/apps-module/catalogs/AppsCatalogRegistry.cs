using RepairService.Db;
using RepairService.Modules.AppsModule.Components;
using RepairService.Modules.AppsModule.Models;
using RepairService.Shared.Contracts;
using System.Collections.Generic;

namespace RepairService.Modules.AppsModule.Catalogs
{
    public static class AppsCatalogRegistry
    {
        public static readonly List<CatalogDefinition> All = new()
        {
            new CatalogDefinition
            {
                ObjectCode = "clients",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS clients (
                        id                  bigserial PRIMARY KEY,
                        last_name           varchar(100) NOT NULL,
                        first_name          varchar(100) NOT NULL,
                        middle_name         varchar(100),
                        phone               varchar(20),
                        email               varchar(255),
                        address             text,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "equipment_types",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS equipment_types (
                        id                  bigserial PRIMARY KEY,
                        name                varchar(100) NOT NULL UNIQUE,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "equipment_brands",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS equipment_brands (
                        id                  bigserial PRIMARY KEY,
                        name                varchar(100) NOT NULL UNIQUE,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "equipment_models",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS equipment_models (
                        id                  bigserial PRIMARY KEY,
                        brand_id            bigint NOT NULL REFERENCES equipment_brands(id),
                        type_id             bigint NOT NULL REFERENCES equipment_types(id),
                        name                varchar(150) NOT NULL,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint,
                        UNIQUE (brand_id, type_id, name)
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "equipment",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS equipment (
                        id                  bigserial PRIMARY KEY,
                        client_id           bigint NOT NULL REFERENCES clients(id),
                        model_id            bigint NOT NULL REFERENCES equipment_models(id),
                        serial_number       varchar(100),
                        purchase_date       date,
                        notes               text,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );",
                CreateIndexesSql = @"
                    CREATE INDEX IF NOT EXISTS idx_equipment_client_id ON equipment(client_id);
                    CREATE INDEX IF NOT EXISTS idx_equipment_model_id  ON equipment(model_id);"
            },

            new CatalogDefinition
            {
                ObjectCode = "order_statuses",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS order_statuses (
                        id                  bigserial PRIMARY KEY,
                        name                varchar(50) NOT NULL UNIQUE,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "order_priorities",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS order_priorities (
                        id                  bigserial PRIMARY KEY,
                        name                varchar(50) NOT NULL UNIQUE,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "services",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS services (
                        id                  bigserial PRIMARY KEY,
                        name                varchar(150) NOT NULL,
                        description         text,
                        base_price          numeric(10,2) NOT NULL CHECK (base_price >= 0),
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "parts",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS parts (
                        id                  bigserial PRIMARY KEY,
                        name                varchar(150) NOT NULL,
                        sku                 varchar(50) UNIQUE,
                        price               numeric(10,2) NOT NULL CHECK (price >= 0),
                        stock_quantity      int NOT NULL DEFAULT 0 CHECK (stock_quantity >= 0),
                        min_stock           int NOT NULL DEFAULT 0 CHECK (min_stock >= 0),
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "orders",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS orders (
                        id                      bigserial PRIMARY KEY,
                        client_id               bigint NOT NULL REFERENCES clients(id),
                        equipment_id            bigint NOT NULL REFERENCES equipment(id),
                        status_id               bigint NOT NULL REFERENCES order_statuses(id),
                        priority_id             bigint NOT NULL REFERENCES order_priorities(id),
                        accepted_at             timestamp NOT NULL DEFAULT now(),
                        planned_end_at          timestamp,
                        completed_at            timestamp,
                        total_cost              numeric(10,2) CHECK (total_cost >= 0),
                        description             text,
                        created_by_user_id      bigint NOT NULL REFERENCES users(id),
                        assigned_engineer_id    bigint REFERENCES employees(id),
                        create_date             timestamp NOT NULL DEFAULT now(),
                        modification_date       timestamp NOT NULL DEFAULT now(),
                        create_user_id          bigint,
                        modification_user_id    bigint
                    );",
                CreateIndexesSql = @"
                    CREATE INDEX IF NOT EXISTS idx_orders_client_id            ON orders(client_id);
                    CREATE INDEX IF NOT EXISTS idx_orders_equipment_id         ON orders(equipment_id);
                    CREATE INDEX IF NOT EXISTS idx_orders_status_id            ON orders(status_id);
                    CREATE INDEX IF NOT EXISTS idx_orders_priority_id          ON orders(priority_id);
                    CREATE INDEX IF NOT EXISTS idx_orders_created_by_user_id   ON orders(created_by_user_id);
                    CREATE INDEX IF NOT EXISTS idx_orders_assigned_engineer_id ON orders(assigned_engineer_id);"
            },

            new CatalogDefinition
            {
                ObjectCode = "order_services",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS order_services (
                        order_id    bigint NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
                        service_id  bigint NOT NULL REFERENCES services(id),
                        quantity    int NOT NULL DEFAULT 1 CHECK (quantity > 0),
                        price       numeric(10,2) NOT NULL CHECK (price >= 0),
                        PRIMARY KEY (order_id, service_id)
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "order_parts",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS order_parts (
                        order_id    bigint NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
                        part_id     bigint NOT NULL REFERENCES parts(id),
                        quantity    int NOT NULL DEFAULT 1 CHECK (quantity > 0),
                        price       numeric(10,2) NOT NULL CHECK (price >= 0),
                        PRIMARY KEY (order_id, part_id)
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "payments",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS payments (
                        id                  bigserial PRIMARY KEY,
                        order_id            bigint NOT NULL REFERENCES orders(id),
                        amount              numeric(10,2) NOT NULL CHECK (amount > 0),
                        payment_date        timestamp NOT NULL DEFAULT now(),
                        payment_method      varchar(50),
                        status              varchar(50),
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );",
                CreateIndexesSql = @"
                    CREATE INDEX IF NOT EXISTS idx_payments_order_id ON payments(order_id);"
            }
        };

        public static IEnumerable<CatalogEntry> Descriptors()
        {
            yield return Entry("clients", "Клиенты", ClientsDescriptor);
            yield return Entry("equipment", "Оборудование", EquipmentDescriptor);
            yield return Entry("orders", "Заказы", OrdersDescriptor);
            yield return Entry("services", "Услуги", ServicesDescriptor);
            yield return Entry("parts", "Запчасти", PartsDescriptor);
        }

        private static CatalogEntry Entry(string code, string title, System.Func<SectionDescriptor> factory) => new()
        {
            ObjectCode = code,
            Title = title,
            DescriptorFactory = factory
        };

        private static SectionDescriptor ClientsDescriptor() => new()
        {
            ObjectCode = "clients",
            Title = "Клиенты",
            LoadData = db => db.SqlQuery(
                @"SELECT id, last_name, first_name, middle_name, phone, email, address
                  FROM clients ORDER BY last_name",
                r => new Client
                {
                    Id = r.GetInt64(0),
                    LastName = r.GetString(1),
                    FirstName = r.GetString(2),
                    MiddleName = r.IsDBNull(3) ? null : r.GetString(3),
                    Phone = r.IsDBNull(4) ? null : r.GetString(4),
                    Email = r.IsDBNull(5) ? null : r.GetString(5),
                    Address = r.IsDBNull(6) ? null : r.GetString(6)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",         Header = "ID",      Type = ColumnType.Number },
                new() { Key = "LastName",   Header = "Фамилия", Type = ColumnType.Text },
                new() { Key = "FirstName",  Header = "Имя",     Type = ColumnType.Text },
                new() { Key = "Phone",      Header = "Телефон", Type = ColumnType.Text },
                new() { Key = "Email",      Header = "Email",   Type = ColumnType.Text },
                new() { Key = "Address",    Header = "Адрес",   Type = ColumnType.Text }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "LastName",   Label = "Фамилия" },
                new() { Key = "FirstName",  Label = "Имя" },
                new() { Key = "MiddleName", Label = "Отчество" },
                new() { Key = "Phone",      Label = "Телефон" },
                new() { Key = "Email",      Label = "Email" },
                new() { Key = "Address",    Label = "Адрес" }
            },
            OnCreate = (db, data) => db.SqlExecute(
                @"INSERT INTO clients (last_name, first_name, middle_name, phone, email, address)
                  VALUES (@ln, @fn, @mn, @ph, @em, @ad)",
                new Dictionary<string, object?>
                {
                    ["@ln"] = data.GetValueOrNull("LastName"),
                    ["@fn"] = data.GetValueOrNull("FirstName"),
                    ["@mn"] = data.GetValueOrNull("MiddleName"),
                    ["@ph"] = data.GetValueOrNull("Phone"),
                    ["@em"] = data.GetValueOrNull("Email"),
                    ["@ad"] = data.GetValueOrNull("Address")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                @"UPDATE clients SET last_name=@ln, first_name=@fn, middle_name=@mn,
                       phone=@ph, email=@em, address=@ad
                  WHERE id = @id",
                new Dictionary<string, object?>
                {
                    ["@ln"] = data.GetValueOrNull("LastName"),
                    ["@fn"] = data.GetValueOrNull("FirstName"),
                    ["@mn"] = data.GetValueOrNull("MiddleName"),
                    ["@ph"] = data.GetValueOrNull("Phone"),
                    ["@em"] = data.GetValueOrNull("Email"),
                    ["@ad"] = data.GetValueOrNull("Address"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM clients WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static SectionDescriptor EquipmentDescriptor() => new()
        {
            ObjectCode = "equipment",
            Title = "Оборудование",
            LoadData = db => db.SqlQuery(
                @"SELECT e.id, e.serial_number, e.purchase_date, c.last_name AS client_name,
                         b.name AS brand_name, m.name AS model_name
                  FROM equipment e
                  JOIN clients c ON c.id = e.client_id
                  JOIN equipment_models m ON m.id = e.model_id
                  JOIN equipment_brands b ON b.id = m.brand_id
                  ORDER BY c.last_name, m.name",
                r => new EquipmentRow
                {
                    Id = r.GetInt64(0),
                    ClientId = r.GetInt64(1),
                    ModelId = r.GetInt64(2),
                    SerialNumber = r.IsDBNull(3) ? null : r.GetString(3),
                    PurchaseDate = r.IsDBNull(4) ? (DateTime?)null : r.GetDateTime(4),
                    ClientName = r.GetString(5),
                    BrandName = r.GetString(6),
                    ModelName = r.GetString(7)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",           Header = "ID",         Type = ColumnType.Number },
                new() { Key = "ClientName",   Header = "Клиент",     Type = ColumnType.Text },
                new() { Key = "BrandName",    Header = "Бренд",      Type = ColumnType.Text },
                new() { Key = "ModelName",    Header = "Модель",     Type = ColumnType.Text },
                new() { Key = "SerialNumber", Header = "Серийный №", Type = ColumnType.Text },
                new() { Key = "PurchaseDate", Header = "Дата покупки", Type = ColumnType.Date }
            },
            FormFields = new List<FormField>()
        };

        private static SectionDescriptor OrdersDescriptor() => new()
        {
            ObjectCode = "orders",
            Title = "Заказы",
            LoadData = db => db.SqlQuery(
                @"SELECT o.id, c.last_name AS client_name, s.name AS status_name,
                         p.name AS priority_name, o.total_cost, o.accepted_at
                  FROM orders o
                  JOIN clients c ON c.id = o.client_id
                  JOIN order_statuses s ON s.id = o.status_id
                  JOIN order_priorities p ON p.id = o.priority_id
                  ORDER BY o.accepted_at DESC",
                r => new OrderRow
                {
                    Id = r.GetInt64(0),
                    ClientId = r.GetInt64(1),
                    EquipmentId = r.GetInt64(2),
                    StatusId = r.GetInt64(3),
                    PriorityId = r.GetInt64(4),
                    ClientName = r.GetString(5),
                    StatusName = r.GetString(6),
                    PriorityName = r.GetString(7),
                    TotalCost = r.IsDBNull(8) ? (decimal?)null : r.GetDecimal(8),
                    AcceptedAt = r.GetDateTime(9)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",           Header = "№",         Type = ColumnType.Number },
                new() { Key = "ClientName",   Header = "Клиент",    Type = ColumnType.Text },
                new() { Key = "StatusName",   Header = "Статус",    Type = ColumnType.Text },
                new() { Key = "PriorityName", Header = "Приоритет", Type = ColumnType.Text },
                new() { Key = "TotalCost",    Header = "Стоимость", Type = ColumnType.Number },
                new() { Key = "AcceptedAt",   Header = "Принят",    Type = ColumnType.Date }
            },
            FormFields = new List<FormField>()
        };

        private static SectionDescriptor ServicesDescriptor() => new()
        {
            ObjectCode = "services",
            Title = "Услуги",
            LoadData = db => db.SqlQuery(
                "SELECT id, name, description, base_price FROM services ORDER BY name",
                r => new ServiceItem
                {
                    Id = r.GetInt64(0),
                    Name = r.GetString(1),
                    Description = r.IsDBNull(2) ? null : r.GetString(2),
                    BasePrice = r.GetDecimal(3)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",          Header = "ID",           Type = ColumnType.Number },
                new() { Key = "Name",        Header = "Наименование", Type = ColumnType.Text },
                new() { Key = "Description", Header = "Описание",     Type = ColumnType.Text },
                new() { Key = "BasePrice",   Header = "Базовая цена", Type = ColumnType.Number }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "Name",        Label = "Наименование" },
                new() { Key = "Description", Label = "Описание" },
                new() { Key = "BasePrice",   Label = "Базовая цена", Kind = FieldKind.Int }
            },
            OnCreate = (db, data) => db.SqlExecute(
                "INSERT INTO services (name, description, base_price) VALUES (@n, @d, @p)",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@d"] = data.GetValueOrNull("Description"),
                    ["@p"] = data.GetValueOrNull("BasePrice")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                "UPDATE services SET name=@n, description=@d, base_price=@p WHERE id=@id",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@d"] = data.GetValueOrNull("Description"),
                    ["@p"] = data.GetValueOrNull("BasePrice"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM services WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static SectionDescriptor PartsDescriptor() => new()
        {
            ObjectCode = "parts",
            Title = "Запчасти",
            LoadData = db => db.SqlQuery(
                "SELECT id, name, sku, price, stock_quantity, min_stock FROM parts ORDER BY name",
                r => new Part
                {
                    Id = r.GetInt64(0),
                    Name = r.GetString(1),
                    Sku = r.IsDBNull(2) ? null : r.GetString(2),
                    Price = r.GetDecimal(3),
                    StockQuantity = r.GetInt32(4),
                    MinStock = r.GetInt32(5)
                }),
            Columns = new List<ColumnDescriptor>
            {
                new() { Key = "Id",             Header = "ID",           Type = ColumnType.Number },
                new() { Key = "Name",           Header = "Наименование", Type = ColumnType.Text },
                new() { Key = "Sku",            Header = "Артикул",      Type = ColumnType.Text },
                new() { Key = "Price",          Header = "Цена",         Type = ColumnType.Number },
                new() { Key = "StockQuantity",  Header = "Остаток",      Type = ColumnType.Number },
                new() { Key = "MinStock",       Header = "Мин. остаток", Type = ColumnType.Number }
            },
            FormFields = new List<FormField>
            {
                new() { Key = "Name",           Label = "Наименование" },
                new() { Key = "Sku",            Label = "Артикул" },
                new() { Key = "Price",          Label = "Цена",          Kind = FieldKind.Int },
                new() { Key = "StockQuantity",  Label = "Остаток",       Kind = FieldKind.Int, Value = 0 },
                new() { Key = "MinStock",       Label = "Мин. остаток",  Kind = FieldKind.Int, Value = 0 }
            },
            OnCreate = (db, data) => db.SqlExecute(
                @"INSERT INTO parts (name, sku, price, stock_quantity, min_stock)
                  VALUES (@n, @s, @p, @q, @m)",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@s"] = data.GetValueOrNull("Sku"),
                    ["@p"] = data.GetValueOrNull("Price"),
                    ["@q"] = data.GetValueOrNull("StockQuantity"),
                    ["@m"] = data.GetValueOrNull("MinStock")
                }),
            OnUpdate = (db, row, data) => db.SqlExecute(
                @"UPDATE parts SET name=@n, sku=@s, price=@p, stock_quantity=@q, min_stock=@m
                  WHERE id=@id",
                new Dictionary<string, object?>
                {
                    ["@n"] = data.GetValueOrNull("Name"),
                    ["@s"] = data.GetValueOrNull("Sku"),
                    ["@p"] = data.GetValueOrNull("Price"),
                    ["@q"] = data.GetValueOrNull("StockQuantity"),
                    ["@m"] = data.GetValueOrNull("MinStock"),
                    ["@id"] = GetValue(row, "Id")
                }),
            OnDelete = (db, row) => db.SqlExecute(
                "DELETE FROM parts WHERE id = @id",
                new Dictionary<string, object?> { ["@id"] = GetValue(row, "Id") })
        };

        private static object? GetValue(object row, string key)
        {
            if (row is IDictionary<string, object?> dict)
                return dict.GetValueOrNull(key);
            var prop = row.GetType().GetProperty(key);
            return prop?.GetValue(row);
        }
    }
}