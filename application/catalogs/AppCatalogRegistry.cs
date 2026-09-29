using System.Collections.Generic;
using RepairService.Db;

namespace RepairService.Application.Catalogs
{
    public static class AppCatalogRegistry
    {
        public static readonly List<CatalogDefinition> All = new()
        {
            new CatalogDefinition
            {
                ObjectCode = "roles",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS roles (
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
                ObjectCode = "user_types",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS user_types (
                        id      bigserial PRIMARY KEY,
                        code    varchar(50) NOT NULL UNIQUE,
                        name    varchar(100) NOT NULL
                    );

                    INSERT INTO user_types (code, name)
                    VALUES ('employee', 'Сотрудник организации'),
                           ('guest',    'Гость')
                    ON CONFLICT (code) DO NOTHING;"
            },

            new CatalogDefinition
            {
                ObjectCode = "user_settings",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS user_settings (
                        id                      bigserial PRIMARY KEY,
                        name                    varchar(100) NOT NULL,
                        can_reset_password      boolean NOT NULL DEFAULT false,
                        block_on_wrong_attempts boolean NOT NULL DEFAULT true,
                        max_wrong_attempts      int NOT NULL DEFAULT 3,
                        auto_unblock_minutes    int NOT NULL DEFAULT 60,
                        password_min_length     int NOT NULL DEFAULT 8,
                        password_digit_length   int NOT NULL DEFAULT 1,
                        password_upper_length   int NOT NULL DEFAULT 0,
                        password_special_length int NOT NULL DEFAULT 0,
                        create_date             timestamp NOT NULL DEFAULT now(),
                        modification_date       timestamp NOT NULL DEFAULT now(),
                        create_user_id          bigint,
                        modification_user_id    bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "employees",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS employees (
                        id                  bigserial PRIMARY KEY,
                        last_name           varchar(100) NOT NULL,
                        first_name          varchar(100) NOT NULL,
                        middle_name         varchar(100),
                        position            varchar(100),
                        phone               varchar(20),
                        email               varchar(255),
                        hire_date           date,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint
                    );"
            },

            new CatalogDefinition
            {
                ObjectCode = "users",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS users (
                        id                       bigserial PRIMARY KEY,
                        code                     varchar(50),
                        last_name                varchar(100) NOT NULL,
                        first_name               varchar(100) NOT NULL,
                        middle_name              varchar(100),
                        email                    varchar(255) NOT NULL UNIQUE,
                        phone                    varchar(20),
                        login                    varchar(50) NOT NULL UNIQUE,
                        password                 varchar(255) NOT NULL,
                        access_banned            boolean NOT NULL DEFAULT false,
                        access_ban_end           timestamp,
                        access_date_start        timestamp,
                        access_date_end          timestamp,
                        role_id                  bigint NOT NULL REFERENCES roles(id),
                        user_type_id             bigint NOT NULL REFERENCES user_types(id),
                        user_setting_id          bigint REFERENCES user_settings(id),
                        employee_id              bigint REFERENCES employees(id),
                        count_wrong_pass_entries int NOT NULL DEFAULT 0,
                        change_password          boolean NOT NULL DEFAULT false,
                        date_change_password     timestamp,
                        last_auth_date           timestamp,
                        first_auth_date          timestamp,
                        comment                  text,
                        avatar                   bytea,
                        create_date              timestamp NOT NULL DEFAULT now(),
                        modification_date        timestamp NOT NULL DEFAULT now(),
                        create_user_id           bigint,
                        modification_user_id     bigint
                    );",
                CreateIndexesSql = @"
                    CREATE INDEX IF NOT EXISTS idx_users_role_id        ON users(role_id);
                    CREATE INDEX IF NOT EXISTS idx_users_user_type_id   ON users(user_type_id);
                    CREATE INDEX IF NOT EXISTS idx_users_user_setting_id ON users(user_setting_id);
                    CREATE INDEX IF NOT EXISTS idx_users_employee_id    ON users(employee_id);"
            },

            new CatalogDefinition
            {
                ObjectCode = "object_claims",
                CreateTableSql = @"
                    CREATE TABLE IF NOT EXISTS object_claims (
                        id                  bigserial PRIMARY KEY,
                        role_id             bigint NOT NULL REFERENCES roles(id) ON DELETE CASCADE,
                        object_code         varchar(100) NOT NULL,
                        can_view            boolean NOT NULL DEFAULT false,
                        can_create          boolean NOT NULL DEFAULT false,
                        can_edit            boolean NOT NULL DEFAULT false,
                        can_delete          boolean NOT NULL DEFAULT false,
                        create_date         timestamp NOT NULL DEFAULT now(),
                        modification_date   timestamp NOT NULL DEFAULT now(),
                        create_user_id      bigint,
                        modification_user_id bigint,
                        UNIQUE (role_id, object_code)
                    );",
                CreateIndexesSql = @"
                    CREATE INDEX IF NOT EXISTS idx_object_claims_role_id    ON object_claims(role_id);
                    CREATE INDEX IF NOT EXISTS idx_object_claims_object_code ON object_claims(object_code);"
            }
        };
    }
}