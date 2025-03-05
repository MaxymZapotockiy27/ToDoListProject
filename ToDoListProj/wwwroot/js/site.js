



// Set the current date
const today = new Date().getDate();
document.getElementById('calendar-date').textContent = today;

// Elements
const sidebar = document.querySelector('.sidebar');
const toggleButton = document.querySelector('.collapse');
const collapseop = document.querySelector('.collapseop');
const inbox = document.querySelector('.inbox');



// Attach event listeners without calling the functions immediately
toggleButton.addEventListener("click", Closesid);
collapseop.addEventListener("click", showUp);

// Function to close sidebar
// Function to close sidebar
function Closesid() {
    const inbox = document.querySelector('.inbox');
    const sidebar = document.querySelector('.sidebar');
    if (inbox && sidebar) { // Check if elements exist
        inbox.style.width = "95%";
        inbox.style.marginLeft = "55px";
        sidebar.style.width = "0px";
        setTimeout(() => {
            const collapseop = document.querySelector('.collapseop');
            if (collapseop) {
                collapseop.style.display = "block";
                collapseop.style.opacity = "1";
            }
        }, 300);
    } else {
        console.error("inbox or sidebar element not found");
    }
}


// Function to show sidebar
function showUp() {
    inbox.style.width = "calc(95% - 311.5px)";
    inbox.style.marginLeft = "335.5px";
    sidebar.style.width = "311.5px";
    setTimeout(() => {
        collapseop.style.display = "none";
    }, 300);
    collapseop.style.opacity = "0";
}

// Media query response
const mediaQuery = window.matchMedia("(max-width: 1150px)");
document.addEventListener("DOMContentLoaded", function () {
    // Отримуємо елементи
    const toggleButton = document.querySelector(".toggle-projtext");
    const projText = document.querySelector(".projhidden-text");

    // Початковий стиль для .projhidden-text
    projText.style.display = "none";  // Приховуємо текст за замовчуванням

    toggleButton.addEventListener("click", function () {
        // Перемикаємо видимість .projhidden-text
        if (projText.style.display === "none") {
            projText.style.display = "block";
        } else {
            projText.style.display = "none";
        }
    });
});

// Media query event handler
function handleMediaChange(e) {
    if (e.matches) {
        Closesid();
    } else {
        showUp();
    }
}

// Add event listener for the media query
mediaQuery.addEventListener("change", handleMediaChange);

// Initial check
handleMediaChange(mediaQuery);
// Отримати поточну дату
const today1 = new Date();
const formattedDate = today1.toLocaleDateString(); // Формат дати: DD.MM.YYYY (залежить від локалі)

// Знайти всі елементи з класом 'current-date' і вставити дату
const dateContainers = document.querySelectorAll('.current-date');
dateContainers.forEach(container => {
    container.textContent = formattedDate;
});


function toggleContent() {
    const content = document.querySelector('.project-content');
    const icon = document.querySelector('.project-icon');
    content.classList.toggle('open');
    icon.classList.toggle('open');
}

function toggleproj(element) {
    // Отримуємо батьківський контейнер проекту
    const project = element.closest('.project');

    // Перевіряємо, чи не натискаємо на одну з кнопок
    if (element.closest('.project-buttons')) {
        return; // Якщо натискаємо на кнопку, припиняємо виконання toggleproj
    }

    // Знаходимо контейнер задач цього проекту
    const tasks = project.querySelector('.project-tasks');

    // Перемикаємо клас 'active' для видимості задач
    tasks.classList.toggle('active');

    // Перемикаємо клас 'open' для іконки (бургер-меню)
    const icon = project.querySelector('.project-burg');
    icon.classList.toggle('open');
}

// Додаємо обробник події для кнопок, щоб зупинити поширення
document.querySelectorAll('.project-buttons img').forEach(button => {
    button.addEventListener('click', function (e) {
        e.stopPropagation(); // Зупиняє поширення події
    });
});


const projectTask = document.querySelector('.project-task');

// Отримуємо чекбокс
const checkbox = document.querySelector('#cbx-12');

// Перевіряємо, чи елементи існують
if (projectTask && checkbox) {
    // Додаємо обробник кліку на div
    projectTask.addEventListener('click', (event) => {
        // Змінюємо стан чекбокса
        checkbox.checked = !checkbox.checked;
    });

    // Додаємо обробник кліку на чекбокс
    checkbox.addEventListener('click', (event) => {
        // Запобігаємо всплиттю події
        event.stopPropagation();
    });
}
document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector('.sidebar');
    const inbox = document.querySelector('.inbox');
    const toggleButton = document.querySelector('.collapse');
    const collapseop = document.querySelector('.collapseop');
    if (sidebar && inbox && toggleButton && collapseop) {
        toggleButton.addEventListener("click", Closesid);
        collapseop.addEventListener("click", showUp);
    }
});

//// Example to dynamically add grouptasks
//const grouptaskContainer = document.querySelector('.grouptasks');
//const grouptaskData = [
//    {
//        image: '~',
//        title: 'Project Title aaaaaaaaaaaaaaaaaaaaaaaaaaaaaa1',
//        description: 'This is a short descriaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaption of the project.',
//        people: ['John Doe', 'Jane Smith'],
//        date: '2025-01-10',
//    },
//    {
//        image: './media/3d-illustration-white-man-with-glasses-bow-tie.jpg',
//        title: 'Project Title 2',
//        description: 'Another description for the second project.',
//        people: ['Alice Brown', 'Bob White'],
//        date: '2025-02-15',
//    },
//    {
//        image: './media/3d-illustration-white-man-with-glasses-bow-tie.jpg',
//        title: 'Project Title 3',
//        description: 'Description for the third project.',
//        people: ['Charlie Green', 'David Black'],
//        date: '2025-03-20',
//    },
//];

//grouptaskData.forEach(({ image, title, description, people, date }) => {
//    const grouptask = `
//    <div class="grouptask">
//        <div class="grouptask-image" style="background-image: url('${image}')"></div>
//        <div class="grouptask-content">
//            <h4 class="grouptask-title">${title}</h4>
//            <p class="grouptask-description">${description}</p>
//            <div class="grouptask-info"><strong>People:</strong> ${people.join(', ')}</div>
//            <div class="grouptask-info"><strong>Date:</strong> ${date}</div>
//        </div>
//    </div>`;
//    grouptaskContainer.insertAdjacentHTML('beforeend', grouptask);


//});


// Отримуємо елемент .notDiv
//const notDiv = document.querySelector('.notDiv');
//const avatarpopdiv = document.querySelector('.avatarpopdiv')
//// Відслідковуємо клік на документі
//document.addEventListener('click', function (event) {
//    // Якщо клік був поза межами .notDiv
//    if (!notDiv.contains(event.target)) {
//        notDiv.style.display = 'none'; // Приховуємо .notDiv
//    }
//});

// Додатково, якщо ви хочете, щоб .notDiv з'являвся за якоюсь подією (наприклад, клік на кнопку)
//function showNotDiv() {
//    notDiv.style.display = 'block'; // Показуємо .notDiv
//}
//document.getElementById('plusbtn').addEventListener('click', function () {
//    document.getElementById('datePickerModal').style.display = 'flex';
//});

