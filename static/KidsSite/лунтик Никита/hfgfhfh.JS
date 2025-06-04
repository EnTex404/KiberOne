Создание простого сайта "Деревенские чаи" с использованием HTML, CSS и JavaScript — это отлично! Давайте создадим базовую структуру этого сайта. Я предоставлю вам пример кода, который включает в себя главную страницу с некоторыми стилями и простым интерактивным элементом.

### HTML (index.html)

```html
<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Деревенские чаи</title>
    <link rel="stylesheet" href="styles.css">
</head>
<body>
    <header>
        <h1>Деревенские чаи</h1>
        <nav>
            <ul>
                <li><a href="#tea">Чаи</a></li>
                <li><a href="#about">О нас</a></li>
                <li><a href="#contact">Контакты</a></li>
            </ul>
        </nav>
    </header>

    <main>
        <section id="tea">
            <h2>Наши чаи</h2>
            <div class="tea-item" onclick="showDetails('черный чай')">
                <h3>Черный чай</h3>
                <p>Ароматный черный чай с лёгкими нотками карамели.</p>
            </div>
            <div class="tea-item" onclick="showDetails('зеленый чай')">
                <h3>Зеленый чай</h3>
                <p>Успокаивающий зеленый чай с травяными добавками.</p>
            </div>
            <div class="tea-item" onclick="showDetails('травяной чай')">
                <h3>Травяной чай</h3>
                <p>Натуральный травяной чай с памятными ароматами.</p>
            </div>
        </section>

        <section id="details" class="hidden">
            <h2>Подробности о чае</h2>
            <p id="tea-description"></p>
            <button onclick="hideDetails()">Закрыть</button>
        </section>

        <section id="about">
            <h2>О нас</h2>
            <p>Мы предлагаем лучшие деревенские чаи, собранные вручную на экологически чистых плантациях.</p>
        </section>

        <section id="contact">
            <h2>Контакты</h2>
            <p>Email: info@derevenskie-chai.ru</p>
        </section>
    </main>

    <footer>
        <p>&copy; 2023 Деревенские чаи. Все права защищены.</p>
    </footer>

    <script src="script.js"></script>
</body>
</html>
```

### CSS (styles.css)

```css
body {
    font-family: Arial, sans-serif;
    margin: 0;
    padding: 0;
    background-color: #f9f9f9;
}

header {
    background-color: #4CAF50;
    color: white;
    padding: 15px;
    text-align: center;
}

nav ul {
    list-style-type: none;
    padding: 0;
}

nav ul li {
    display: inline;
    margin: 0 15px;
}

nav a {
    color: white;
    text-decoration: none;
}

main {
    padding: 20px;
}

.tea-item {
    background-color: white;
    border: 1px solid #ddd;
    border-radius: 5px;
    padding: 10px;
    margin: 10px 0;
    cursor: pointer;
    transition: background-color 0.3s;
}

.tea-item:hover {
    background-color: #f1f1f1;
}

.hidden {
    display: none;
}

footer {
    text-align: center;
    padding: 10px;
    background-color: #4CAF50;
    color: white;
    position: relative;
}
```

### JavaScript (script.js)

```javascript
function showDetails(tea) {
    let description = "";
    
    switch (tea) {
        case 'черный чай':
            description = "Ароматный черный чай с лёгкими нотками карамели.";
            break;
        case 'зеленый чай':
            description = "Успокаивающий зеленый чай с травяными добавками.";
            break;
        case 'травяной чай':
            description = "Натуральный травяной чай с памятными ароматами.";
            break;
    }
    
    document.getElementById("tea-description