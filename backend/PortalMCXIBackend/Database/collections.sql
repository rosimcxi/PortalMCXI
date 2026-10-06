-- =====================================================================
-- Soubor: collections.sql
-- Projekt: PortalMCXI
-- Verze: 1.0.0
-- Datum: 2026-10-06
-- Ucel: Datovy model modulu Sbirky. Aplikace je rizena samostatne;
--       tento soubor se NESPOUSTI automaticky pri startu API.
-- =====================================================================

CREATE SCHEMA IF NOT EXISTS collections;

CREATE TABLE IF NOT EXISTS collections.item (
    item_id uuid PRIMARY KEY,
    item_type text NOT NULL,
    name text NOT NULL,
    country text NULL,
    year integer NULL,
    denomination text NULL,
    quantity integer NOT NULL DEFAULT 1 CHECK (quantity > 0),
    condition text NULL,
    catalog_number text NULL,
    metal text NULL,
    fineness numeric(8,4) NULL,
    weight_g numeric(12,4) NULL,
    diameter_mm numeric(12,4) NULL,
    purchase_price numeric(14,2) NULL,
    purchase_currency char(3) NOT NULL DEFAULT 'CZK',
    purchase_date date NULL,
    acquisition_source text NULL,
    storage_location text NULL,
    notes text NULL,
    identity_confidence numeric(5,2) NULL,
    identity_status text NOT NULL DEFAULT 'UNVERIFIED',
    created_at timestamptz NOT NULL DEFAULT now(),
    updated_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_collection_item_type ON collections.item(item_type);
CREATE INDEX IF NOT EXISTS ix_collection_item_metal ON collections.item(metal);
CREATE INDEX IF NOT EXISTS ix_collection_item_year ON collections.item(year);

CREATE TABLE IF NOT EXISTS collections.photo (
    photo_id uuid PRIMARY KEY,
    item_id uuid NOT NULL REFERENCES collections.item(item_id) ON DELETE CASCADE,
    photo_type text NOT NULL,
    storage_key text NOT NULL,
    sha256 char(64) NULL,
    width integer NULL,
    height integer NULL,
    ai_description text NULL,
    is_primary boolean NOT NULL DEFAULT false,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_collection_photo_item ON collections.photo(item_id);

CREATE TABLE IF NOT EXISTS collections.market_observation (
    observation_id uuid PRIMARY KEY,
    item_id uuid NOT NULL REFERENCES collections.item(item_id) ON DELETE CASCADE,
    observed_at timestamptz NOT NULL,
    source_name text NOT NULL,
    source_url text NULL,
    observation_type text NOT NULL,
    price numeric(14,2) NULL,
    currency char(3) NULL,
    sold_at timestamptz NULL,
    evidence_text text NULL,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_collection_observation_item_date
    ON collections.market_observation(item_id, observed_at DESC);

CREATE TABLE IF NOT EXISTS collections.valuation (
    valuation_id uuid PRIMARY KEY,
    item_id uuid NOT NULL REFERENCES collections.item(item_id) ON DELETE CASCADE,
    valued_at timestamptz NOT NULL,
    metal_value numeric(14,2) NULL,
    market_min numeric(14,2) NULL,
    market_estimate numeric(14,2) NULL,
    market_max numeric(14,2) NULL,
    currency char(3) NOT NULL DEFAULT 'CZK',
    confidence numeric(5,2) NULL,
    method text NULL,
    ai_provider text NULL,
    ai_model text NULL,
    source_count integer NOT NULL DEFAULT 0,
    created_at timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX IF NOT EXISTS ix_collection_valuation_item_date
    ON collections.valuation(item_id, valued_at DESC);

CREATE OR REPLACE VIEW collections.v_item_current_value AS
SELECT
    i.item_id,
    i.item_type,
    i.name,
    i.country,
    i.year,
    i.denomination,
    i.quantity,
    i.metal,
    i.fineness,
    i.weight_g,
    i.purchase_price,
    i.purchase_currency,
    v.valued_at,
    v.metal_value,
    v.market_min,
    v.market_estimate,
    v.market_max,
    v.currency AS valuation_currency,
    v.confidence
FROM collections.item i
LEFT JOIN LATERAL (
    SELECT x.*
    FROM collections.valuation x
    WHERE x.item_id = i.item_id
    ORDER BY x.valued_at DESC
    LIMIT 1
) v ON true;
