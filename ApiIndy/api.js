const  express = require('express')

const app =  express();

app.use((request, response, next) => {
    response.header('Access-Control-Allow-Origin', '*');
    response.header('Access-Control-Allow-Headers', 'Authorization, X-API-KEY, Origin, X-Requested-With, Content-Type, Accept, Access-Control-Allow-Request-Method');
    response.header('Access-Control-Allow-Methods', 'GET, POST, OPTIONS, PUT, DELETE');
    response.header('Allow', 'GET, POST, OPTIONS, PUT, DELETE');
    next();
});



app.get('/',(request,response,next)=>{
        response.send('Api Indy');

})

app.get('/CrearWallet',(request,response,next)=>{
    let data = {"did" : "ALMNC-1235-ALDJK" ,"nombre": "Victor Adrian Reyes"}
    response.json(data);

})

app.post('/CrearWallet',(request,response,next)=>{
    let data = {"did" : "ALMNC-1235-ALDJK" ,"nombre": "Victor Adrian Reyes"}
    response.json(data);

})

//Para levantar el servidor



app.listen(3030,function(){
    console.log("Servidor listo ...!! ");
})

//instalar el servidor y se quede ejecutandose
// npm install -g nodemon
// nodemon api.js