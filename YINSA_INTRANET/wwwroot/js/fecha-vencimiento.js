
	$(document).ready(function () {

		let tdFechas = document.querySelectorAll('.fecha-vencimiento');
		let fechaActual = new Date();
			tdFechas.forEach((f) =>{
				var fechaString = f.innerText;

				var partesFecha = fechaString.split('/');

				var fechaVencimiento= new Date(partesFecha[2], partesFecha[1] - 1, partesFecha[0]);
				

				if (fechaActual > fechaVencimiento)
				{
					console.log("red")
					f.classList.add('red');
				}

				let porVencer = new Date(f.innerText);

				porVencer.setDate(porVencer.getDate() - 7);

				if (fechaActual >= porVencer && fechaActual < fechaVencimiento)
				{
					console.log("yellow")
					f.classList.add('yellow');
				}

			});

			


		})
