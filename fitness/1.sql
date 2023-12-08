CREATE TABLE IF NOT EXISTS lift (
    lift_name TEXT PRIMARY KEY,
    description_l TEXT,
    video TEXT,
    image BLOB
);

CREATE TABLE IF NOT EXISTS routine (
    routine_number INT PRIMARY KEY,
    routine_name TEXT,
    description_r TEXT
);

CREATE TABLE IF NOT EXISTS is_in (
    lift_name TEXT NOT NULL,
    routine_num INT NOT NULL,
    weight INT,
    repetitions INT,
    PRIMARY KEY (lift_name, routine_num),
    FOREIGN KEY (lift_name)
        REFERENCES lift (lift_name),
    FOREIGN KEY (routine_num)
        REFERENCES routine (routine_number)
);

CREATE TABLE IF NOT EXISTS muscle_group (
    lift_name TEXT NOT NULL,
    group_name TEXT NOT NULL,
    PRIMARY KEY (lift_name, group_name),
    FOREIGN KEY (lift_name)
        REFERENCES lift (lift_name)
);