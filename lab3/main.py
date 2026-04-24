import sqlite3

conn = sqlite3.connect('lab3/parking.db')
cursor = conn.cursor()

def CreateTables():
    query = """
        CREATE TABLE IF NOT EXISTS Cars (
            License_Plate TEXT PRIMARY KEY,
            Car_Type      TEXT,
            Client_ID     INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Parking_Access (
            License_Plate TEXT NOT NULL,
            Work_Date     TEXT NOT NULL,
            Access_Permit INTEGER NOT NULL,
            PRIMARY KEY (License_Plate, Work_Date),
            FOREIGN KEY (License_Plate) REFERENCES Cars(License_Plate)
        );

        CREATE TABLE IF NOT EXISTS Payments (
            Entry_ID      INTEGER PRIMARY KEY,
            License_Plate TEXT NOT NULL,
            Work_Date     TEXT NOT NULL,
            Entry_Time    TEXT NOT NULL,
            Entry_Type    TEXT,
            FOREIGN KEY (License_Plate, Work_Date) REFERENCES Parking_Access(License_Plate, Work_Date)
        );
    """
    cursor.executescript(query)

def InsertData():
    query = """
        INSERT INTO Cars VALUES
            ('A123AA77',  'Sedan',      110),
            ('B555BB99',  'SUV',        102),
            ('C333CC199', 'SUV',        107),
            ('E777EE177', 'Sedan',      103),
            ('H888HH777', 'Motorcycle', 106),
            ('K001KK750', 'Truck',      104),
            ('M444MM97',  'SUV',        105),
            ('P999PP77',  'Truck',      108),
            ('T111TT99',  'Sedan',      109),
            ('X222XX50',  'Sedan',      101);
    """
    cursor.executemany(query)

    query = """
        INSERT INTO Parking_Access VALUES
            ('A123AA77',  '2025-08-27', 1),
            ('B555BB99',  '2025-06-02', 1),
            ('C333CC199', '2025-11-25', 0),
            ('E777EE177', '2025-10-15', 1),
            ('H888HH777', '2026-01-14', 1),
            ('K001KK750', '2025-12-17', 0),
            ('M444MM97',  '2025-11-19', 1),
            ('P999PP77',  '2025-10-22', 1),
            ('T111TT99',  '2025-09-24', 1),
            ('X222XX50',  '2025-10-21', 1);
    """
    cursor.executemany(query)

    query = """
        INSERT INTO Payments VALUES
            (1,  'A123AA77',  '2025-08-27', '08:30', 'Single'),
            (2,  'B555BB99',  '2025-06-02', '09:50', 'Subscription'),
            (3,  'E777EE177', '2025-10-15', '10:09', 'Single'),
            (4,  'H888HH777', '2026-01-14', '11:22', 'Subscription'),
            (5,  'M444MM97',  '2025-11-19', '08:25', 'Single'),
            (6,  'P999PP77',  '2025-10-22', '08:45', 'Subscription'),
            (7,  'T111TT99',  '2025-09-24', '13:06', 'Single'),
            (8,  'X222XX50',  '2025-10-21', '01:07', 'Subscription'),
            (9,  'A123AA77',  '2025-08-27', '18:04', 'Single'),
            (10, 'B555BB99',  '2025-06-02', '20:00', 'Single');
    """
    cursor.executemany(query)

conn.commit()
conn.close()
print("Done")