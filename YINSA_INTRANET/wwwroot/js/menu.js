

const ham = document.querySelector('.ham');
const enlaces = document.querySelector('.enlaces-menu');
const barras = document.querySelectorAll('.ham span');
const nav = document.getElementById("menu-nav");
const logo= document.querySelector('.logo');

ham.addEventListener('click', () => {
   /* console.log("click");*/
    enlaces.classList.toggle('activado');
    nav.classList.toggle('activado');
    logo.classList.toggle('activado');
    barras.forEach(child => { child.classList.toggle('animado') });
    barras.forEach(child => { child.classList.toggle('activado') });
    ham.classList.toggle('girar');

    if (window.innerWidth > 1171) {
        ham.classList.add('off');
    }

});
window.addEventListener("scroll", function (event) {
    enlaces.classList.remove('activado');
    ham.classList.remove('off');
    /*console.log('remove nav activado');*/
    nav.classList.remove('activado');
    barras.forEach(child => { child.classList.remove('activado') });
});

window.addEventListener('resize', function () {
    barras.forEach(child => { child.classList.remove('animado') });
    enlaces.classList.remove('activado');
    nav.classList.remove('activado');
});



