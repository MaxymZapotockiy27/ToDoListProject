$(document).on("click", ".delete-icon", function () {
    let taskId = $(this).data("task-id");
    let taskElement = $(this).closest(".task-container"); // Знайти контейнер завдання

    $.ajax({
        url: "/ToDoList/DeleteTask",
        type: "POST",
        data: { taskId: taskId },
        success: function (response) {
            if (response.success) {
                console.log("Task deleted successfully!");
                taskElement.fadeOut(150, function () { $(this).remove(); }); // Плавне видалення
            }
        },
        error: function (xhr, status, error) {
            console.error("Error deleting task:", error);
        }
    });
});


$(document).ready(function () {
    $(document).on("change", ".cbx input[type='checkbox']", function () {
        let taskId = $(this).data("task-id");
        let isCompleted = $(this).prop("checked");
        $.ajax({
            url: "/ToDoList/UpdateTaskStatus",
            type: "POST",
            data: { taskId: taskId, isCompleted: isCompleted },
            success: function (response) {
                console.log("Task updated successfully!");
            },
            error: function (xhr, status, error) {
                console.error("Error updating task:", error);
            }
        });
    });
});

$(document).ready(function () {
    $(document).on("click", ".task-text, .edit-icon", function () {
        let taskElement = $(this).closest(".task-container").find(".task-text");
        let taskId = $(this).closest(".task-container").find(".edit-icon").data("task-id");

        if (taskElement.find("input").length > 0) return;

        let currentText = taskElement.text();
        let inputField = $("<input>", {
            type: "text",
            value: currentText,
            class: "task-input"
        });

        taskElement.html(inputField);
        inputField.focus();

        inputField[0].setSelectionRange(inputField.val().length, inputField.val().length);

        inputField.on("keypress blur", function (e) {
            if (e.type === "blur" || e.which === 13) {
                let newText = inputField.val().trim();
                if (newText === "") {
                    taskElement.text(currentText);
                    return; 
                }
                if (newText !== currentText) {
                    $.ajax({
                        url: "/ToDoList/UpdateTaskTitle",
                        type: "POST",
                        data: { taskId: taskId, newTitle: newText },
                        success: function (response) {
                            taskElement.text(newText);
                            console.log("Task title updated successfully!");
                        },
                        error: function (xhr, status, error) {
                            console.error("Error updating task:", error);
                        }
                    });
                } else {
                    taskElement.text(currentText);
                }
            }
        });
    });
});
$(document).ready(function () {
    $(document).on("click", ".delete-btn", function () {
        let taskId = $(this).closest(".project-task").find("input[type='checkbox']").data("task-id");
        let taskElement = $(this).closest(".project-task");

        $.ajax({
            url: "/ToDoList/DeleteTask",
            type: "POST",
            data: { taskId: taskId },
            success: function (response) {
                if (response.success) {
                    console.log("Task deleted successfully!");
                    taskElement.fadeOut(150, function () { $(this).remove(); }); 
                }
            },
            error: function (xhr, status, error) {
                console.error("Error deleting task:", error);
            }
        });
    });

    $(document).on("click", ".edit-btn, .project-task-label", function () {
        let taskElement = $(this).closest(".project-task").find(".project-task-label");
        let taskId = $(this).closest(".project-task").find("input[type='checkbox']").data("task-id");

        if (taskElement.find("input").length > 0) return;

        let currentText = taskElement.text().trim();
        let inputField = $("<input>", {
            type: "text",
            value: currentText,
            class: "task-input"
        });

        taskElement.html(inputField);
        inputField.focus();

        inputField[0].setSelectionRange(inputField.val().length, inputField.val().length);

        inputField.on("keypress blur", function (e) {
            if (e.type === "blur" || e.which === 13) {
                let newText = inputField.val().trim();
                if (newText === "") {
                    taskElement.text(currentText); 
                    return;
                }
                if (newText !== currentText) {
                    $.ajax({
                        url: "/ToDoList/UpdateTaskTitle",
                        type: "POST",
                        data: { taskId: taskId, newTitle: newText },
                        success: function (response) {
                            taskElement.text(newText);
                            console.log("Task title updated successfully!");
                        },
                        error: function (xhr, status, error) {
                            console.error("Error updating task:", error);
                        }
                    });
                } else {
                    taskElement.text(currentText);
                }
            }
        });
    });
});
$(document).ready(function () {
    $(".ProjtaskForm").on("submit", function (e) {
        e.preventDefault(); 

        var form = $(this);
        var formData = form.serialize();

    
        $.ajax({
            type: "POST",
            url: "/ToDoList/CreateProjectTask",
            data: formData,
            success: function (response) {
                var projectTasksContainer = form.closest(".project").find(".project-tasks");
                var imagetrashPath = "/media/trash-plus-alt-svgrepo-com (1).svg";
                var imageeditPath = "/media/edit-2-svgrepo-com (1).svg";
                projectTasksContainer.append(`
                <div class="project-task">
                    <div class="project-task-label">
                        ${response.title}
                    </div>
                    <div class="task-buttons">
                        <img src="${imagetrashPath}" class="delete-btn" alt="Delete">
                        <img src="${imageeditPath}" class="edit-btn" alt="Edit">
                    </div>
                    <div class="checkbox-wrapper-12">
                        <div class="cbx">
                            <input id="task-${response.id}" type="checkbox" data-task-id="${response.id}" ${response.IsCompleted ? "checked" : ""} />
                            <label for="task-${response.id}"></label>
                            <svg width="15" height="14" viewbox="0 0 15 14" fill="none">
                                <path d="M2 8.36364L6.23077 12L13 2"></path>
                            </svg>
                        </div>
                        <!-- Gooey -->
                        <svg xmlns="http://www.w3.org/2000/svg" version="1.1">
                            <defs>
                                <filter id="goo-@projecttasks.Id">
                                    <fegaussianblur in="SourceGraphic" stddeviation="4" result="blur"></fegaussianblur>
                                    <fecolormatrix in="blur" mode="matrix" values="1 0 0 0 0  0 1 0 0 0  0 0 1 0 0  0 0 0 22 -7" result="goo"></fecolormatrix>
                                    <feblend in="SourceGraphic" in2="goo"></feblend>
                                </filter>
                            </defs>
                        </svg>
                    </div>
                </div>
                `);

                form.find("input[name='Name']").val("");
            },
            error: function () {
                alert("Error adding task. Please try again.");
            }
        });
    });
});
$(document).ready(function () {


    $(".deleteproject-btn").click(function () {
        var projectId = $(this).data("project-id");
        var projectElement = $(this).closest(".project");

        $.ajax({
            url: '/ToDoList/DeleteProject',
            type: 'POST',
            data: { projectId: projectId },
            success: function (response) {
                if (response.success) {
                    console.log("Project deleted successfully!");
                    projectElement.fadeOut(150, function () { $(this).remove(); });
                }
            },
            error: function (xhr, status, error) {
                console.error("Error deleting project:", error);
            }
        });
    });
});

$(document).ready(function () {
    $(".edit-project-btn").click(function () {
        var projectId = $(this).data("project-id");
        var currentTitle = $(this).closest(".project-in").find(".proj-name").text();
        var elementTitle = $(this).closest(".project-in").find(".proj-name");

        $("#editProjectModal").fadeIn();  
        $("#newTitle").val(currentTitle);  

     
        $("#editProjectModal button:contains('Save')").off('click').on('click', function () {
            var newTitle = $("#newTitle").val();
            $.ajax({
                url: "/ToDoList/UpdateProjectTitle",
                type: "POST",
                data: { name: newTitle, projectId: projectId },
                success: function (response) {
                    if (response.success) {
                        elementTitle.text(newTitle);
                        $("#editProjectModal").fadeOut();
                    } else {
                        alert("Error updating project");
                    }
                },
                error: function () {
                    alert("An error occurred.");
                }
            });
        });

  
        $("#editProjectModal button:contains('Cancel')").click(function () {
            $("#editProjectModal").fadeOut();
        });
    });
});

