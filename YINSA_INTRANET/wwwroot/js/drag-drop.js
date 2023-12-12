const dropArea = document.querySelector(".drag-area");
const dragText = dropArea.querySelector(".drag-text");
//const button = dropArea.querySelector("button");
const button = document.querySelector(".show-files");
//const input = dropArea.querySelector("#input-file");
const input = document.getElementById('input-file');
const inputDesc = document.getElementById('input-desc');
const inputComment = document.getElementById('input-comentario');
const inputEntry = document.getElementById('input-entry');
const inputNum = document.getElementById('input-num');
const inputObj= document.getElementById('input-obj');

let files;
let archivos = new Array();


//FUNCIONES PARA MODAL
button.addEventListener('click', (e) => {
    
    input.click();
});

input.addEventListener('change', (e) => {
    /*  files = this.files;*/
    files = e.target.files;
    console.log(files);
    // dropArea.classList.add("active");
    showFiles(files);
    //dropArea.remove("active");
});

dropArea.addEventListener("dragover", (e) => {
    e.preventDefault();
    dropArea.classList.add("active");
    dragText.textContent = "Suelte para cargar los archivos"
});

dropArea.addEventListener("dragleave", (e) => {
    e.preventDefault();
    dropArea.classList.remove("active");
    dragText.textContent = "Arrastre y suelte los archivos";
});

dropArea.addEventListener("drop", (e) => {
    e.preventDefault();
    files = e.dataTransfer.files;

    showFiles(files);
    dropArea.classList.remove("active");
    dragText.textContent = "Arrastre y suelte los archivos";
});

function showFiles(files) {
    if (files.length === undefined) {
        processFile(files);
    } else {
        for (const file of files) {
            processFile(file);
        }
    }
}

function processFile(file) {

    const container = document.querySelector('#preview');
   
    const docType = file.type;
    const validExtensions = ['application/pdf', 'application/xml', 'text/xml'];
    /* const validExtensions = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];*/

    const validFileExtensions = ['.pdf', '.xml'];

    const fileExtension = file.name.substring(file.name.lastIndexOf('.')).toLowerCase();
    
    if (validExtensions.includes(docType) || validFileExtensions.includes(fileExtension)) {
      
        const fileReader = new FileReader();
        const id = `file-${Math.random().toString(32).substring(7)}`;
        fileReader.addEventListener('load', e => {
            const fileUrl = fileReader.result;

            let archivo = {
                id: id,
                name: file.name,
                data: fileUrl,
                type: docType
            };

            archivos.push(archivo);
            container.innerHTML = '';
            generarFiles()
           
        });

        fileReader.readAsDataURL(file);
       
    }
    else {
        alert('Debe ingresar los archivos pdf y xml de la factura.');
    }
}

async function uploadFile(url) {
    let req = {
        docEntry: inputEntry.value,
        docNum: inputNum.value,
        obj: inputObj.value,
        descripcion: inputDesc.value,
        comentario: inputComment.value,
        files: archivos
    }

    console.log(req);

    console.log(JSON.stringify(req));

    var modalContent = document.querySelector("#modalInfo");

    const text = document.createElement('p');
    text.textContent = 'Subiendo factura . . .';

    modalContent.innerHTML = '';
    modalContent.appendChild(text);

        await fetch(url, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(req)

        })
            .then((response) => {
                    console.log(response);

                  
                    if (!response.ok) {
                        
                        response.json().then(error => {
                            
                            text.textContent = error.message
                        });
                        modalContent.innerHTML = '';
                        modalContent.appendChild(text);
                    }
                    else {
                        modalContent.innerHTML = '';
                        text.textContent = 'Archivos subidos correctamente.';
                        modalContent.appendChild(text);

                        archivos = [];
                        inputDesc.value = '';
                        inputComment.value='';
                        document.querySelector('#preview').innerHTML = '';
                    }
            })
            .catch((error) => {
                console.log(error)
                modalContent.innerHTML = '';
                text.textContent = 'Ha ocurrido un problema. Inténtelo de nuevo';
                modalContent.appendChild(text);
            });

       

}

function popFile(id) {

    archivos = archivos.filter(a => a.id != id);
    const container = document.querySelector('#preview');
    container.innerHTML = '';
    generarFiles();
    
}

function generarFiles() {

    for (var archivo of archivos) {

        var tipo = (archivo.type == 'application/pdf' ? '/img/icons/pdf.png' : '/img/icons/xml.png');

        const image = `
              <div id="${archivo.id}" class="file"  data-toggle="tooltip" data-placement="bottom" title="${archivo.name}">
                     <div class="file-img">
                       <span class="delete-file" onclick="popFile('${archivo.id}')">
                           <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle" viewBox="0 0 16 16">
                               <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z" />
                               <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z" />
                           </svg>
                       </span>
                       <img src="${tipo}" />
                     </div>             
              </div>
            `;

        const html = document.querySelector('#preview').innerHTML;
        document.querySelector('#preview').innerHTML = image + html;
    }
}




                               /*
                               const image = `
              <div id="${archivo.id}" class="file"  data-toggle="tooltip" data-placement="bottom" title="${archivo.name}">
                     <div class="file-img">
                       <span class="delete-file" onclick="popFile('${archivo.id}')">
                           <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-x-circle" viewBox="0 0 16 16">
                               <path d="M8 15A7 7 0 1 1 8 1a7 7 0 0 1 0 14zm0 1A8 8 0 1 0 8 0a8 8 0 0 0 0 16z" />
                               <path d="M4.646 4.646a.5.5 0 0 1 .708 0L8 7.293l2.646-2.647a.5.5 0 0 1 .708.708L8.707 8l2.647 2.646a.5.5 0 0 1-.708.708L8 8.707l-2.646 2.647a.5.5 0 0 1-.708-.708L7.293 8 4.646 5.354a.5.5 0 0 1 0-.708z" />
                           </svg>
                       </span>
                       <img src="${tipo}" />
                     </div>
                     <span>${archivo.name}</span>               
              </div>
            `;
                               
                               
                               
                               
                               */