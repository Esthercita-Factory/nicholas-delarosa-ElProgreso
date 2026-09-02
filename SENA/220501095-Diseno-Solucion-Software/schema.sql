-- Modelo relacional para "Cooperativa Financiera El Progreso"
-- Respalda el mismo dominio implementado en Models/Member.cs y Models/Movement.cs
-- si IMemberRepository se implementara contra una base de datos (hoy es en memoria).
-- Compatible con PostgreSQL; para MySQL/SQL Server ajustar tipos de fecha e identity.

CREATE TABLE members (
    document_number VARCHAR(10)  PRIMARY KEY,
    full_name       VARCHAR(150) NOT NULL,
    phone_number    VARCHAR(20)  NULL,
    address         VARCHAR(200) NULL,
    joined_at       TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT ck_members_document_number_format
        CHECK (document_number ~ '^[0-9]{6,10}$')
);

CREATE TABLE movements (
    id                      BIGSERIAL     PRIMARY KEY,
    member_document_number  VARCHAR(10)   NOT NULL,
    type                    VARCHAR(10)   NOT NULL,
    amount                  DECIMAL(15,2) NOT NULL,
    fee                     DECIMAL(15,2) NOT NULL DEFAULT 0,
    occurred_at             TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_movements_member
        FOREIGN KEY (member_document_number)
        REFERENCES members (document_number)
        ON DELETE RESTRICT,               -- espeja la regla: no se puede eliminar un asociado con movimientos

    CONSTRAINT ck_movements_type
        CHECK (type IN ('Deposit', 'Withdrawal')),

    CONSTRAINT ck_movements_amount_positive
        CHECK (amount > 0),               -- espeja "no se aceptan movimientos <= 0"

    CONSTRAINT ck_movements_fee_valid
        CHECK (fee IN (0, 8000))          -- espeja la comisión fija de manejo de efectivo
);

CREATE INDEX ix_movements_member_document_number ON movements (member_document_number);
CREATE INDEX ix_movements_occurred_at             ON movements (occurred_at);

-- Vista de solo lectura para acelerar los informes gerenciales.
-- No reemplaza la validación de negocio (saldo insuficiente, comisión condicional):
-- esa lógica sigue viviendo en la capa de aplicación (Member.RegisterWithdrawal).
CREATE VIEW member_balances AS
SELECT
    m.document_number,
    m.full_name,
    COALESCE(SUM(
        CASE mv.type
            WHEN 'Deposit'    THEN mv.amount
            WHEN 'Withdrawal' THEN -(mv.amount + mv.fee)
        END
    ), 0) AS balance
FROM members m
LEFT JOIN movements mv ON mv.member_document_number = m.document_number
GROUP BY m.document_number, m.full_name;
