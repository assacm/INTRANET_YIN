'use strict';

import options from './json/menu.json' assert { type: 'json'}

//console.log(options);
//export let menuOptions = options;

export async function customMenu(url) {
    let opt = options;
    
    let res = await fetch(url);
    const typeUser = await res.json();


    const menu = document.querySelector('.enlaces-menu');
    let li1 = menu.getElementsByTagName('li')[0];

    opt[typeUser.user].opciones.forEach((opcion,i) => {
        console.log(opcion);

        let enlace = document.createElement('a');
        enlace.href = opcion.ruta;
        enlace.textContent = opcion.nombre;

        let li = document.createElement('li');
        li.appendChild(enlace);
        if (i > 0) {
            li1 = menu.getElementsByTagName('li')[0];
            li1.insertAdjacentElement('afterend',li);
        } else {
            menu.insertBefore(li, li1);
        }
 
      
        // Inserta el nuevo li antes del primer li existente
       
      
    });

}

//customMenu();