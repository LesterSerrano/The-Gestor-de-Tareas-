//Función par cerrar modales
function hideAllModals() {
    closeModal("createModal");
    closeModal("editModal");
    closeModal("detailsModal");
    closeModal("deleteModal");
}

function handleResponse(response, isError = false) {

    hideAllModals();

    if (isError || (response && response.success === false)) {
        let errorMessage = isError ? 'Error al procesar la solicitud.' : (response.message || 'Error desconocido.');

        showCustomAlert(errorMessage, 'error');

    }
    else if (response && response.success) {

        $.get("/Usuario/Index", function (html) {
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

function submiteditForm(formId) {
    var formData = $('#' + formId).serialize();
    $.ajax({
        url: '/Usuario/Edit',
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

function eliminarUsuario() {
    $.ajax({
        url: '/Usuario/Delete',
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

function cargarVistaEdit(id) {
    $.ajax({
        url: "/Usuario/Edit/" + id,
        type: "GET",
        success: function (data) {
            document.getElementById("editModalContent").innerHTML = data;
            openModal("editModal");
        }
    });
}

function cargarVistaDetails(id) {
    $.ajax({
        url: "/Usuario/Details/" + id,
        type: "GET", // Solicitud para obtener la vista
        success: function (data) {
            document.getElementById("detailsModalContent").innerHTML = data;
            openModal("detailsModal");
        }
    });
}

function cargarVistaDelete(id) {
    $.ajax({
        url: "/Usuario/Delete/" + id,
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