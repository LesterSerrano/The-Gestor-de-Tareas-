//Función par cerrar modales
function hideAllModals() {
    closeModal("createModal");
    closeModal("editModal");
    closeModal("detailsModal");
    closeModal("deleteModal");
}

function handleResponse(response, isError = false) {

    hideAllModals();
    // Cerrar el modal y luego mostrar el mensaje
    setTimeout(() => {
        if (isError) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Hubo un problema al procesar la solicitud.',
                confirmButtonText: 'OK',
                showConfirmButton: true,
                timer: null
            });
        } else {
            Swal.fire({
                icon: response.success ? 'success' : 'error',
                title: response.success ? 'Éxito' : 'Error',
                text: response.message,
                timer: response.success ? 2000 : null,
                showConfirmButton: false
            }).then(() => {
                if (response.success) {
                    $.get("/Cargo/Index", function (html) {
                        let nuevoContenido = $(html).find("#tablaActualizablePosi").html();
                        $("#tablaActualizablePosi").html(nuevoContenido);
                    });
                }
            });
        }
    }, 300);
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
        url: '/Cargo/Create',
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
        url: '/Cargo/Edit',
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

function eliminarCargo() {
    $.ajax({
        url: '/Cargo/Delete',
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
        url: "/Cargo/Create",
        type: "GET",
        success: function (data) {
            document.getElementById("createModalContent").innerHTML = data;
            openModal("createModal");

        }
    });
}

function cargarVistaEdit(id) {
    $.ajax({
        url: "/Cargo/Edit/" + id,
        type: "GET",
        success: function (data) {
            document.getElementById("editModalContent").innerHTML = data;
            openModal("editModal");

        }
    });
}

function cargarVistaDetails(id) {
    $.ajax({
        url: "/Cargo/Details/" + id,
        type: "GET",
        success: function (data) {
            document.getElementById("detailsModalContent").innerHTML = data;
            openModal("detailsModal");
        }
    });
}

function cargarVistaDelete(id) {
    $.ajax({
        url: "/Cargo/Delete/" + id,
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