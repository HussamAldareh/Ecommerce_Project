
```javascript id="q9tp2m"
console.log("ADMIN JS LOADED");
```
    

/* ── LUXORA CORE CONFIGURATION ────────────────────── */
const pageMeta = {
    dashboard: ['Dashboard', 'Admin / Dashboard'],
    products: ['Products', 'Admin / Catalogue / Products'],
    categories: ['Categories', 'Admin / Catalogue / Categories'],
    orders: ['Orders', 'Admin / Commerce / Orders'],
    users: ['Users', 'Admin / Commerce / Users'],
    testimonials: ['Testimonials', 'Admin / Moderation / Testimonials'],
    comments: ['Reviews & Ratings', 'Admin / Moderation / Reviews'],
    settings: ['Settings', 'Admin / Settings'],
};

// Global Notification Helper
function notify(type, title, message) {
    Swal.fire({
        toast: true,
        position: 'top-end',
        icon: type, // 'success', 'error', 'warning', 'info'
        title: title,
        text: message,
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true,
        background: '#004225', // Emerald
        color: '#f8fafc',      // White
        iconColor: '#d4af37'   // Gold
    });
}

/* ── NAVIGATION & UI ────────────────────────────── */
function showPage(id, btn) {
    document.querySelectorAll('.page-section').forEach(s => s.classList.remove('active'));
    document.querySelectorAll('.nav-link-item').forEach(l => l.classList.remove('active'));

    const page = document.getElementById('page-' + id);
    if (page) page.classList.add('active');
    if (btn) btn.classList.add('active');

    const m = pageMeta[id] || [id, 'Admin / ' + id];
    document.getElementById('topbar-title').textContent = m[0];
    document.getElementById('topbar-crumb').textContent = m[1];
    document.getElementById('sidebar').classList.remove('open');
}

/* ── MODALS ──────────────────────────────────────── */
function openModal(id) {
    const modal = document.getElementById(id);
    if (modal) modal.classList.add('open');
}

function closeModal(id) {
    const modal = document.getElementById(id);
    if (modal) modal.classList.remove('open');
}

// Close modal when clicking outside (on the overlay)
document.querySelectorAll('.modal-overlay').forEach(o => {
    o.addEventListener('click', e => {
        if (e.target === o) o.classList.remove('open');
    });
});

/* ── TABLE UTILITIES ────────────────────────────── */
function filterTbl(tableId, query) {
    const q = query.toLowerCase().trim();
    document.querySelectorAll('#' + tableId + ' tbody tr').forEach(r => {
        const text = (r.getAttribute('data-search') || '') + ' ' + r.innerText.toLowerCase();
        r.style.display = !q || text.includes(q) ? '' : 'none';
    });
}

function toggleAll(master, tableId) {
    document.querySelectorAll('#' + tableId + ' tbody input[type="checkbox"]')
        .forEach(cb => cb.checked = master.checked);
}

/* ── SLUG GENERATOR ──────────────────────────────── */
function genSlug(input) {
    const slugInput = document.getElementById('cat-slug-input');
    if (slugInput) {
        slugInput.value = input.value
            .toLowerCase()
            .trim()
            .replace(/\s+/g, '-')
            .replace(/[^a-z0-9-]/g, '');
    }
}

function vaultSearch(query) {
    const q = query.toLowerCase().trim();
    // This targets every row in whichever table is currently on screen
    document.querySelectorAll('.lx-table tbody tr').forEach(row => {
        const text = (row.getAttribute('data-search') || '') + ' ' + row.innerText.toLowerCase();
        row.style.display = !q || text.includes(q) ? '' : 'none';
    });
}

/* ── INITIALIZATION ──────────────────────────────── */
//document.addEventListener("DOMContentLoaded", function () {
//    // Welcome Greeting
//    setTimeout(() => {
//        notify('info', 'System Access', 'Welcome back to the LUXORA Vault, Admin!');
//    }, 700);
//});



document.addEventListener("DOMContentLoaded", function () {

    // Welcome toast
    setTimeout(() => {
        notify('info', 'System Access', 'Welcome back to the LUXORA Vault, Admin!');
    }, 700);

    // Revenue Chart
    const chartCanvas = document.getElementById('revenueChart');

    if (chartCanvas) {
        new Chart(chartCanvas, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'],
                datasets: [{
                    label: 'Revenue',
                    data: [1200, 2100, 1800, 3200, 4200, 5100],
                    borderColor: '#d4af37',
                    backgroundColor: 'rgba(212,175,55,.15)',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                plugins: {
                    legend: {
                        labels: {
                            color: '#f8fafc'
                        }
                    }
                },
                scales: {
                    x: {
                        ticks: {
                            color: '#cbd5e1'
                        }
                    },
                    y: {
                        ticks: {
                            color: '#cbd5e1'
                        }
                    }
                }
            }
        });
    }

});