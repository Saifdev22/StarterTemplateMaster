/*=============== SHOW MENU ===============*/
window.showMenu = (toggleId, navId) => {
    console.log("From JS");
    const toggle = document.getElementById(toggleId),
        nav = document.getElementById(navId);

    toggle.addEventListener('click', () => {
        nav.classList.toggle('show-menu');
        toggle.classList.toggle('show-icon');
    });
};
