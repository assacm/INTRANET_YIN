var spinner = document.getElementById('spinner');
var error = document.getElementById('view-error');

function load() {
	spinner.classList.toggle('disable');
	error.textContent = '';
}