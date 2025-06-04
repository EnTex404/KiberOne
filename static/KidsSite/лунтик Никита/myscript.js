// app.js
document.addEventListener("DOMContentLoaded", () => {
    const propertyForm = document.getElementById('propertyForm');
    const propertiesList = document.getElementById('propertiesList');

    // Получаем список объявлений из LocalStorage
    function getProperties() {
        return JSON.parse(localStorage.getItem('properties')) || [];
    }

    // Отображаем все объявления
    function displayProperties() {
        const properties = getProperties();
        propertiesList.innerHTML = '';
        properties.forEach((property, index) => {
            const propertyDiv = document.createElement('div');
            propertyDiv.className = 'property';
            propertyDiv.innerHTML = `
                <h3>${property.name}</h3>
                <p>Цена: ${property.price} руб.</p>
                <button onclick="deleteProperty(${index})">Удалить</button>
            `;
            propertiesList.appendChild(propertyDiv);
        });
    }

    // Добавляем новое объявление
    propertyForm.addEventListener('submit', (e) => {
        e.preventDefault();
        const propertyName = document.getElementById('propertyName').value;
        const propertyPrice = document.getElementById('propertyPrice').value;

        const properties = getProperties();
        properties.push({ name: propertyName, price: propertyPrice });
        localStorage.setItem('properties', JSON.stringify(properties));
        propertyForm.reset();
        displayProperties();
    });

    // Удаляем объявление
    window.deleteProperty = (index) => {
        const properties = getProperties();
        properties.splice(index, 1);
        localStorage.setItem('properties', JSON.stringify(properties));
        displayProperties();
    };

    displayProperties();
});


