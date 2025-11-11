
function generateAlertHtml(message, type) {
    const colors = {
        success: {
            bg: 'bg-green-100 dark:bg-green-900',
            border: 'border-green-500 dark:border-green-700',
            text: 'text-green-900 dark:text-green-100',
            icon: 'text-green-600',
            hover: 'hover:bg-green-200 dark:hover:bg-green-800'
        },
        error: {
            bg: 'bg-red-100 dark:bg-red-900',
            border: 'border-red-500 dark:border-red-700',
            text: 'text-red-900 dark:text-red-100',
            icon: 'text-red-600',
            hover: 'hover:bg-red-200 dark:hover:bg-red-800'
        }
    };
    const style = colors[type] || colors.error;

    return `
        <div id="tempAlert" role="alert"
             class="${style.bg} border-l-4 ${style.border} ${style.text} p-2 rounded-lg flex items-center transition duration-300 ease-in-out ${style.hover} transform hover:scale-105">
            <svg stroke="currentColor" viewBox="0 0 24 24" fill="none" class="h-6 w-6 flex-shrink-0 mr-3 ${style.icon}" xmlns="http://www.w3.org/2000/svg">
                <path d="M13 16h-1v-4h1m0-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" stroke-width="2" stroke-linejoin="round" stroke-linecap="round"></path>
            </svg>
            <p class="text-sm font-semibold">${message}</p>
        </div>
    `;
}

function showCustomAlert(message, type) {
    const alertContainer = document.getElementById('alertContainer');
    if (!alertContainer) return;

    alertContainer.innerHTML = '';
    alertContainer.innerHTML = generateAlertHtml(message, type);

    const tempAlert = document.getElementById('tempAlert');
    if (tempAlert) {
        setTimeout(() => {
            tempAlert.style.transition = 'opacity 0.5s ease';
            tempAlert.style.opacity = '0';
            setTimeout(() => tempAlert.remove(), 500);
        }, 4000);
    }
}