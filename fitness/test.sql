CREATE TABLE IF NOT EXISTS progress (
    routine_number INT NOT NULL,
    image BLOB NOT NULL,
    date TEXT,
    PRIMARY KEY (routine_number, image),
    FOREIGN KEY (routine_number)
        REFERENCES routine (routine_number)
);