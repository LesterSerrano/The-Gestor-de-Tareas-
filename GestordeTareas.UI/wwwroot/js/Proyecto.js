function hideAllModals() {
    closeModal("createModal");
    closeModal("editModal");
    closeModal("detailsModal");
    closeModal("deleteModal");
}

function toggleMenu(id) {
    var menu = document.getElementById('menu-' + id);
    if (menu.classList.contains('hidden')) {
        menu.classList.remove('hidden');
    } else {
        menu.classList.add('hidden');
    }
}

window.addEventListener('click', function (e) {
    var allMenus = document.querySelectorAll('[id^="menu-"]');

    allMenus.forEach(function (menu) {
        if (!menu.contains(e.target) && !document.getElementById('menuButton-' + menu.id.split('-')[1]).contains(e.target)) {
            menu.classList.add('hidden');
        }
    });
});

function handleResponse(response, isError = false) {

    hideAllModals();

    if (isError || (response && response.success === false)) {
        let errorMessage = isError ? 'Error al procesar la solicitud.' : (response.message || 'Error desconocido.');

        showCustomAlert(errorMessage, 'error');

    }
    else if (response && response.success) {

        $.get("/Proyecto/Index", function (html) {
            let nuevoContenido = $(html).find("#tablaActualizablePosi").html();
            $("#tablaActualizablePosi").html(nuevoContenido);

            showCustomAlert(response.message, 'success');
        });
    }
}

function openModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;

    modal.classList.remove("hidden");
    modal.classList.add("flex");

    // Bloquear scroll del body
    document.body.classList.add("overflow-hidden");
}


function closeModal(id) {
    const modal = document.getElementById(id);
    if (!modal) return;

    modal.classList.add("hidden");
    modal.classList.remove("flex");

    // Rehabilitar scroll del body
    document.body.classList.remove("overflow-hidden");
}

function submitcreateForm(formId) { 
    var formData = $('#' + formId).serialize();  
    $.ajax({ 
        url: '/Proyecto/Create', 
        type: 'POST',  
        data: formData, 
        success: function (response) {
            handleResponse(response);
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            handleResponse(null, true); 
        }
    });
}

function submiteditForm(formId) { 
    var formData = $('#' + formId).serialize(); 
    $.ajax({ 
        url: '/Proyecto/Edit', 
        type: 'POST',
        data: formData, 
        success: function (response) {
            handleResponse(response);
        },
        error: function (xhr, status, error) {
            console.error(xhr.responseText);
            handleResponse(null, true);
        }
    });
}

function eliminarProyecto() { 
    $.ajax({ 
        url: '/Proyecto/Delete', 
        type: 'POST', 
        data: $("#deleteForm").serialize(), 
        success: function (response) { 
            handleResponse(response); 
        },
        error: function (xhr, status, error) { 
            console.error(xhr.responseText);
            handleResponse(null, true); 
        }
    });
}

function cargarVistaCreate() { 
    $.ajax({ 
        url: "/Proyecto/Create", 
        type: "GET", 
        success: function (data) { 
            document.getElementById("createModalContent").innerHTML = data;
            openModal("createModal"); 
        }
    });
}

function cargarVistaEdit(id) { 
    $.ajax({ 
        url: "/Proyecto/Edit/" + id, 
        type: "GET",
        success: function (data) { 
            document.getElementById("editModalContent").innerHTML = data;
            openModal("editModal"); 
        }
    });
}

function cargarVistaDelete(id) { 
    $.ajax({ 
        url: "/Proyecto/Delete/" + id, 
        type: "GET", 
        success: function (data) { 
            document.getElementById("deleteModalContent").innerHTML = data;
            openModal("deleteModal"); 
        },
        error: function () { 
            alert("Error al cargar la vista de eliminación."); 
        }
    });
}

// Evita submit tradicional al presionar Enter
$(document).on('keypress', '#createForm, #editForm', function (e) {
    if (e.key === 'Enter') {
        e.preventDefault();

        if (this.id === 'createForm') submitcreateForm('createForm');
        else if (this.id === 'editForm') submiteditForm('editForm');
    }
});
