
        // Lógica del modo oscuro (Botón) 
        const darkModeToggle = document.getElementById('darkModeToggle');

        if (darkModeToggle) {
            darkModeToggle.addEventListener('click', () => {
                const isDarkMode = document.body.classList.toggle('dark');
                localStorage.setItem('darkMode', isDarkMode ? 'dark' : 'light');

                const darkModeIcon = document.getElementById('darkModeIcon');
                if (darkModeIcon) {
                    darkModeIcon.innerHTML = isDarkMode
                        ? '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" class="text-white" style="fill: #ffffff;"><path d="M12 11.807A9.002 9.002 0 0 1 10.049 2a9.942 9.942 0 0 0-5.12 2.735c-3.905 3.905-3.905 10.237 0 14.142 3.906 3.906 10.237 3.905 14.143 0a9.946 9.946 0 0 0 2.735-5.119A9.003 9.003 0 0 1 12 11.807z"></path></svg>'
                        : '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="bi bi-sun-fill text-yellow-500" viewBox="0 0 16 16"><path d="M8 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8M8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0m0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13m8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5M3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8m10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0m-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0m9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707M4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708"/></svg>';
                }
            });
        }

        window.addEventListener('load', () => {
             const savedMode = localStorage.getItem('darkMode');
             const darkModeIcon = document.getElementById('darkModeIcon');
             if (darkModeIcon) {
                 darkModeIcon.innerHTML = savedMode === 'dark'
                     ? '<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" class="text-white" style="fill: #ffffff;"><path d="M12 11.807A9.002 9.002 0 0 1 10.049 2a9.942 9.942 0 0 0-5.12 2.735c-3.905 3.905-3.905 10.237 0 14.142 3.906 3.906 10.237 3.905 14.143 0a9.946 9.946 0 0 0 2.735-5.119A9.003 9.003 0 0 1 12 11.807z"></path></svg>'
                     : '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="bi bi-sun-fill text-yellow-500" viewBox="0 0 16 16"><path d="M8 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8M8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0m0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13m8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5M3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8m10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.415a.5.5 0 1 1-.707-.708l1.414-1.414a.5.5 0 0 1 .707 0m-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0m9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707M4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .708"/></svg>';
             }
        });


        // Lógica del Sidebar/Menú de Navegación
        document.addEventListener('DOMContentLoaded', function () {
            const menuButton = document.getElementById('menuButton');
            const sidebar = document.getElementById('drawer-navigation');

            if (menuButton && sidebar) {
                const closeButton = document.querySelector('[data-drawer-hide="drawer-navigation"]');

                menuButton.addEventListener('click', (event) => {
                    event.stopPropagation();
                    sidebar.style.transform = 'translateX(0)';
                });

                closeButton.addEventListener('click', () => {
                    sidebar.style.transform = 'translateX(-100%)';
                });

                document.addEventListener('click', (event) => {
                    if (!sidebar.contains(event.target) && !menuButton.contains(event.target)) {
                        sidebar.style.transform = 'translateX(-100%)';
                    }
                });

                const sidebarLinks = document.querySelectorAll('#drawer-navigation a');
                sidebarLinks.forEach(link => {
                    link.addEventListener('click', () => {
                        setTimeout(() => {
                            sidebar.style.transform = 'translateX(-100%)';
                        }, 100);
                    });
                });
            }
        });


        // Lógica del Menú de Usuario
        document.addEventListener('DOMContentLoaded', function () {
            const userMenuButton = document.getElementById('userMenuButton');
            const userMenuOptions = document.getElementById('userMenuOptions');

            if (userMenuButton && userMenuOptions) {
                userMenuButton.addEventListener('click', function () {
                    userMenuOptions.style.display = userMenuOptions.style.display === 'block' ? 'none' : 'block';
                });

                userMenuOptions.addEventListener('click', function (event) {
                    if (event.target.tagName === 'A') {
                        userMenuOptions.style.display = 'none';
                        event.stopPropagation();
                    }
                });

                document.addEventListener('click', function (event) {
                    if (!userMenuButton.contains(event.target) && !userMenuOptions.contains(event.target)) {
                        userMenuOptions.style.display = 'none';
                    }
                });
            }
        });


        // Lógica del Loader
        document.addEventListener("DOMContentLoaded", function () {
            const loaderOverlay = document.getElementById('loader-overlay');

            function showLoader() {
                if (loaderOverlay) { 
                    loaderOverlay.classList.add('active');
                }
            }

            function hideLoader() {
                if (loaderOverlay) { 
                    loaderOverlay.classList.remove('active');
                }
            }

            // Detectar clics en todos los enlaces y formularios
            document.body.addEventListener('click', function (event) {
                const target = event.target;
                const link = target.closest('a');

                // Excluir los elementos que no deben activar el loader (como los del menú de usuario/dark mode/sidebar toggle)
                if (
                    target.closest('[onclick*="cargarVista"]') ||
                    target.closest('form[action*="DeleteOwn"]') ||
                    target.closest('[onclick="toggleEditMode()"]') ||
                    (target.closest('form') && target.closest('form').action.includes('/Comment/Create')) ||
                    target.closest('#darkModeToggle') || 
                    target.closest('#menuButton') || 
                    target.closest('#userMenuButton') 
                ) {
                    return;
                }

                // Activar el loader para enlaces internos
                if (link && link.hasAttribute('href') && !link.getAttribute('target') && link.getAttribute('href') !== '#') {

                    showLoader();

                }


                // Activar el loader para botones de envío de formularios
                if (target.tagName === 'BUTTON' || (target.tagName === 'INPUT' && target.type === 'submit')) {
                    const form = target.closest('form');
                    if (form) {
                        showLoader();
                    }
                }
            });

            // Ocultar el loader cuando la página termine de cargar
            window.addEventListener('load', hideLoader);
            window.addEventListener('beforeunload', showLoader);
        });
