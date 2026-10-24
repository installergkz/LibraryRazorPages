USE master;
GO

-- Удалить БД, если существует
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'Library')
BEGIN
    ALTER DATABASE [Library] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [Library];
    PRINT 'БД Library удалена.';
END
GO

-- Создать БД
CREATE DATABASE [Library];
PRINT 'БД Library создана.';
GO

USE [Library];
GO

-- Таблица PublicationHouses
IF OBJECT_ID(N'dbo.PublicationHouses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.PublicationHouses
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(250) NOT NULL,
        Note NVARCHAR(1000) NULL,

        CONSTRAINT PK_PublicationHouses PRIMARY KEY CLUSTERED (Id)
    );
END
GO

-- Таблица Books
IF OBJECT_ID(N'dbo.Books', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Books
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Name NVARCHAR(250)  NOT NULL,
        Authors NVARCHAR(250)  NOT NULL,
        PublicationYear INT NULL,
        PublicationHouseId INT NULL,
        PagesCount INT NULL,
        ISBN NVARCHAR(25) NULL,
        Note NVARCHAR(1000) NULL,
        ContentsList XML NULL,

        CONSTRAINT PK_Books PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT UQ_Books_ISBN UNIQUE (ISBN),
        CONSTRAINT FK_Books_PublicationHouses
            FOREIGN KEY (PublicationHouseId)
            REFERENCES dbo.PublicationHouses (Id)
    );
END
GO


-- Хранимые процедуры
-- GetBooks
CREATE OR ALTER PROCEDURE dbo.GetBooks
    @Id INT = NULL,
    @ISBN NVARCHAR(25) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        b.Id, b.[Name], b.Authors, b.PublicationYear, b.PagesCount,
        b.ISBN, b.Note, b.ContentsList, b.PublicationHouseId,
        h.Id, h.[Name], h.[Note]
    FROM dbo.Books b
    LEFT JOIN dbo.PublicationHouses h ON h.Id = b.PublicationHouseId
    WHERE (@Id IS NULL OR b.Id = @Id) AND (@ISBN IS NULL OR b.ISBN = @ISBN);
END;
GO

-- CreateBook
CREATE OR ALTER PROCEDURE dbo.CreateBook
    @Name NVARCHAR(250),
    @Authors NVARCHAR(250),
    @PublicationYear INT = NULL,
    @PublicationHouseId INT = NULL,
    @PagesCount INT = NULL,
    @ISBN NVARCHAR(25) = NULL,
    @Note NVARCHAR(1000) = NULL,
    @ContentsList XML = NULL,
    @NewBookId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Books
    (
        [Name], Authors, PublicationYear, PublicationHouseId,
        PagesCount, ISBN, Note, ContentsList
    )
    VALUES
    (
        @Name, @Authors, @PublicationYear, @PublicationHouseId,
        @PagesCount, @ISBN, @Note, @ContentsList
    );

    SET @NewBookId = SCOPE_IDENTITY();
END;
GO

-- UpdateBook
CREATE OR ALTER PROCEDURE dbo.UpdateBook
    @Id INT,
    @Name NVARCHAR(250) = NULL,
    @Authors NVARCHAR(250) = NULL,
    @PublicationYear INT = NULL,
    @PublicationHouseId INT = NULL,
    @PagesCount INT = NULL,
    @ISBN NVARCHAR(25) = NULL,
    @Note NVARCHAR(1000) = NULL,
    @ContentsList XML = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Books SET
        [Name] = COALESCE(@Name, [Name]),
        Authors = COALESCE(@Authors, Authors),
        PublicationYear = COALESCE(@PublicationYear, PublicationYear),
        PublicationHouseId = COALESCE(@PublicationHouseId, PublicationHouseId),
        PagesCount = COALESCE(@PagesCount, PagesCount),
        ISBN = COALESCE(@ISBN, ISBN),
        Note = COALESCE(@Note, Note),
        ContentsList = COALESCE(@ContentsList, ContentsList)
    WHERE Id = @Id;
END;
GO

-- DeleteBook
CREATE OR ALTER PROCEDURE dbo.DeleteBook
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Books WHERE Id = @Id;
END;
GO


-- GetPublicationHouses
CREATE OR ALTER PROCEDURE dbo.GetPublicationHouses
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, [Name], Note
    FROM dbo.PublicationHouses
    WHERE (@Id IS NULL OR Id = @Id)
    ORDER BY [Name];
END;
GO


-- Отчеты
-- Все заголовки книг
CREATE OR ALTER PROCEDURE dbo.GetBookHeadings
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.[Name], T.c.value('.', 'nvarchar(max)') AS Heading
    FROM dbo.Books b
    CROSS APPLY b.ContentsList.nodes('/content/h2') AS T(c)
    WHERE b.ContentsList IS NOT NULL
    ORDER BY b.Id, T.c.value('.', 'nvarchar(max)');
END;
GO

-- Количество заголовков по книгам
CREATE OR ALTER PROCEDURE dbo.GetHeadingCounts
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, [Name], ContentsList.value('count(/content/h2)', 'int') AS HeadingCount
    FROM dbo.Books
    WHERE ContentsList IS NOT NULL
    ORDER BY HeadingCount DESC, Id;
END;
GO

-- Первый заголовок каждой книги
CREATE OR ALTER PROCEDURE dbo.GetFirstHeadings
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Id, [Name], ContentsList.value('(/content/h2[1])[1]', 'nvarchar(max)') AS FirstHeading
    FROM dbo.Books
    WHERE ContentsList IS NOT NULL
    ORDER BY Id;
END;
GO

-- Поиск книг по заголовку
CREATE OR ALTER PROCEDURE dbo.SearchBooksByHeading
    @pattern NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT b.Id, b.[Name], T.c.value('.', 'nvarchar(max)') AS Heading
    FROM dbo.Books b
    CROSS APPLY b.ContentsList.nodes('/content/h2') AS T(c)
    WHERE b.ContentsList IS NOT NULL
      AND (@pattern IS NULL OR T.c.value('.', 'nvarchar(max)') LIKE @pattern)
    ORDER BY b.Id, T.c.value('.', 'nvarchar(max)');
END;
GO

-- Заполнение таблиц
-- Издательства
INSERT INTO dbo.PublicationHouses (Name, Note) VALUES
    (N'Просвещение', NULL),
    (N'Эксмо-АСТ', NULL),
    (N'Росмэн', NULL),
    (N'Азбука-Аттикус', NULL),
    (N'Экзамен', NULL),
    (N'Лань', NULL),
    (N'Манн, Иванов и Фербер', NULL),
    (N'Альпина Паблишер', NULL),
    (N'Махаон', NULL),
    (N'Феникс', NULL),
    (N'Клевер-Медиа-Групп', NULL),
    (N'Питер', NULL);

-- Книги
INSERT INTO dbo.Books
    (Name, Authors, PublicationYear, PublicationHouseId, PagesCount, ISBN, Note, ContentsList)
VALUES
    (N'Незнайка на Луне',
     N'Носов Николай',
     2024,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Махаон'),
     480,
     N'978-5-389-19117-4',
     N'«Незнайка на Луне» – заключительная часть трилогии Николая Носова о приключениях знаменитого и любимого детьми коротышки из Цветочного города. На этот раз Незнайка вместе с Пончиком тайком пробирается на ракету, которую их друзья подготовили для экспедиции на Луну. Случайно запустив двигатель, они вдвоём улетают на спутник Земли, где знакомятся с лунатиками и оказываются в центре захватывающих событий.',
     NULL),

    (N'Война и мир, том 1',
     N'Толстой Лев',
     2024,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Азбука-Аттикус'),
     700,
     N'978-5-389-21519-1',
     N'Роман Льва Николаевича Толстого «Война и мир» — книга на все времена. Кажется, что она существовала всегда, настолько знакомым кажется текст, едва мы открываем первые страницы романа, настолько памятны многие его эпизоды: охота и святки, первый бал Наташи Ростовой, лунная ночь в Отрадном, князь Андрей в сражении при Аустерлице... Сцены «мирной», семейной жизни сменяются картинами, имеющими значение для хода всей мировой истории, но для Толстого они равноценны, связаны в едином потоке времени. Каждый эпизод важен не только для развития сюжета, но и как одно из бесчисленных проявлений жизни, которая насыщена в каждом своем моменте и которую учит любить Толстой.  В данном издании воспроизводится полный комплект иллюстраций к роману «Война и мир», созданных выдающимся чешским художником Станиславом Гудечеком (1872–1947).',
     NULL),
     
     (N'Война и мир, том 2',
     N'Толстой Лев',
     2024,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Азбука-Аттикус'),
     742,
     N'978-5-389-21520-7',
     N'Роман Льва Николаевича Толстого «Война и мир» — книга на все времена. Кажется, что она существовала всегда, настолько знакомым кажется текст, едва мы открываем первые страницы романа, настолько памятны многие его эпизоды: охота и святки, первый бал Наташи Ростовой, лунная ночь в Отрадном, князь Андрей в сражении при Аустерлице... Сцены «мирной», семейной жизни сменяются картинами, имеющими значение для хода всей мировой истории, но для Толстого они равноценны, связаны в едином потоке времени. Каждый эпизод важен не только для развития сюжета, но и как одно из бесчисленных проявлений жизни, которая насыщена в каждом своем моменте и которую учит любить Толстой.  В данном издании воспроизводится полный комплект иллюстраций к роману «Война и мир», созданных выдающимся чешским художником Станиславом Гудечеком (1872–1947).',
     NULL),
     
     (N'Властелин колец',
     N'Джон Рональд Руэл Толкин',
     2021,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Эксмо-АСТ'),
     1696,
     N'978-5-17-092791-3',
     N'«Властелин колец» — эпическое фэнтези о судьбе Средиземья, где исход большой войны зависит не только от силы правителей и армий, но и от ноши, которую должен вынести один небольшой герой. В этом мире рядом существуют разные народы, и у каждого свои предания, язык, история и представление о чести. Книга связана с «Хоббитом» общей вселенной, но заметно превосходит его по масштабу, драматизму и глубине проработки мира. В это издание вошёл перевод В. Каррика и М. Каменкович — один из известных русских вариантов прочтения знаменитой трилогии.',
     NULL),
     
     (N'JavaScript. Рецепты для разработчиков. 3-е изд',
     N'Адам Д. Скотт, Мэтью Макдоналд',
     2026,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Питер'),
     528,
     N'978-5-4461-2001-7',
     N'Зачем изобретать велосипед, сталкиваясь с очередной проблемой в JavaScript? Здесь вы найдете множество рецептов кода для типовых задач программирования, а также познакомитесь c методами создания приложений, работающих в любом браузере. Адаптируемые примеры кода можно вставить практически в любой проект, а заодно поглубже изучить JS. С помощью этой книги вы научитесь настраивать эффективную среду разработки с редактором кода, статическим анализатором и тестовым сервером; станете лучше понимать функции JS, включая замыкания и генераторы; узнаете, как использовать классы и наследование — основные концепции ООП; освоите работу с мультимедиа, включая аудио, видео и SVG; научитесь управлять HTML и CSS; благодаря Node.js сможете использовать JavaScript где угодно; узнаете, как получать доступ к удаленным данным и управлять ими с помощью технологий REST, GraphQL и Fetch; освоите популярную среду разработки приложений Express; научитесь выполнять асинхронные операции с промисами, async/await.',
     NULL),

     (N'Теория игр. Искусство стратегического мышления в бизнесе и жизни',
     N'Авинаш Диксит',
     2026,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Манн, Иванов и Фербер'),
     464,
     N'978-5-00195-207-7',
     N'«Теория игр. Искусство стратегического мышления в бизнесе и жизни» — это книга о стратегическом мышлении, конкуренции, сотрудничестве и принятии решений в ситуациях, где результат зависит не только от одного человека. Авинаш Диксит объясняет теорию игр без сложной математики и показывает, как её принципы работают в бизнесе, политике, спорте, истории и повседневных выборах. Это чтение для тех, кто хочет лучше понимать логику переговоров, соперничества, сотрудничества и скрытых правил любой «игры», в которой участвуют люди, компании или целые системы.  В центре внимания — ситуации, где каждый шаг зависит от действий других участников. Автор показывает, как устроены стратегические решения, почему одни ходы усиливают позицию, а другие загоняют в тупик, и как меняется выбор, если в игре есть конфликт интересов, союзники, риск или нехватка информации. В книге последовательно разбираются основные понятия теории игр: дерево игры, доминирующие и доминируемые стратегии, равновесие Нэша, смешанны',
     NULL),

     (N'Букварь',
     N'Надежда Жукова',
     2025,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Эксмо-АСТ'),
     96,
     N'978-5-699-90986-5',
     N'Знаменитый «Букварь» Надежды Жуковой — учебное пособие, выпущенное в стандартном формате, удобном для всех детских ручек. Это пособие уже более 30 лет помогает детям быстро и эффективно учиться соединять отдельные буквы в слоги и слова и овладевать техникой чтения без ошибок. «Букварь» рекомендован Министерством общего и профессионального образования РФ и соответствует ФГОС. Общий тираж «Букваря» — более 12 млн экземпляров.',
     NULL),

     (N'Одиссея',
     N'Гомер',
     2024,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Эксмо-АСТ'),
     416,
     N'978-5-17-115443-1',
     N'История хитроумного и бесстрашного Одиссея, пережившего множество удивительных и опасных приключений на пути домой из Трои, и его верной жены, прекрасной и сильной духом Пенелопы, известна всем – кровожадные циклопы и сладкоголосые сирены, мудрая чародейка Цирцея и таинственные лотофаги… Ни одно произведение мировой литературы не вызывало столько споров и дискуссий, не ложилось в основу такого немыслимого количества научных теорий, как «Одиссея» Гомера. Откуда взялся так поражающий исследователей «женский взгляд» в «Одиссее»? Зашифровал ли Гомер, в образе «морского» народа феакхов, обитателей минойского Крита, а если да – то откуда знал о них? Вот лишь немногие из вопросов, на которые только предстоит ответить читателям и исследователям загадочного шедевра Гомера…',
     NULL),

     (N'Божественная комедия',
     N'Данте Алигьери',
     2026,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Эксмо-АСТ'),
     512,
     N'978-5-04-230835-2',
     N'«Божественная комедия» — величайшее творение Данте Алигьери, вершина поэтического и философского гения Средневековья. В грандиозном путешествии через Ад, Чистилище и Рай поэт проходит путь очищения и обретения истины, осмысляя судьбу человеческой души и законы мироздания. Объединив религию, науку и политику, Данте создал символическую и глубоко личную поэму о человеке, его грехах, страданиях и вечном стремлении к Богу. В странствиях вместе с Вергилием и Беатриче герой встречает великих деятелей прошлого, размышляет о добре и зле, о божественной справедливости и предназначении человека — и восходит из мрака к свету.',
     NULL),

     (N'Евгений Онегин',
     N'Александр Пушкин',
     2026,
     (SELECT Id FROM dbo.PublicationHouses WHERE Name = N'Эксмо-АСТ'),
     256,
     N'978-5-04-225981-4',
     N'Культовый роман в стихах «Евгений Онегин» — это энциклопедия пушкинской эпохи, наполненная фразеологизмами, цитатами, намеками и аллюзиями, выискивать которые — настоящее удовольствие для читателя. Главный герой — щеголь и повеса, которому к двадцати шести годам успевают наскучить шумные балы и городская суета, получает в наследство от покойного дяди поместье и состояние и поселяется в деревенской глуши. Здесь он мгновенно становится предметом для сплетен и пересудов. А его высокомерие больно ранит влюбленную в него Татьяну Ларину.',
     NULL);

-- Проверка
SELECT 
    (SELECT COUNT(*) FROM dbo.PublicationHouses) AS HousesCount,
    (SELECT COUNT(*) FROM dbo.Books) AS BooksCount;
GO

SELECT name FROM sys.procedures ORDER BY name;
GO