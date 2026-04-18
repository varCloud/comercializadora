const axios = require('axios');
const fs = require('fs');
const path = require('path');
const xml = require('xml-parser');

const BASE_URL = 'https://www.sat.gob.mx/sitio_internet/cfd/4/cadenaoriginal_4_0/';
const MAIN_FILE = 'cadenaoriginal_4_0.xslt';
const DOWNLOAD_DIR = './descargas';
const LOG_FILE = './descarga.log';

let downloadedFiles = new Set();
let failedFiles = [];

function log(message) {
  const timestamp = new Date().toISOString();
  const logMessage = `[${timestamp}] ${message}`;
  console.log(logMessage);
  fs.appendFileSync(LOG_FILE, logMessage + '\n');
}

async function ensureDir() {
  if (!fs.existsSync(DOWNLOAD_DIR)) {
    fs.mkdirSync(DOWNLOAD_DIR, { recursive: true });
    log(`Directorio creado: ${DOWNLOAD_DIR}`);
  }
}

async function downloadFile(fileUrl, fileName) {
  if (downloadedFiles.has(fileName)) {
    log(`⊘ Archivo ya descargado: ${fileName}`);
    return true;
  }

  try {
    const response = await axios.get(fileUrl, {
      timeout: 10000
    });
    
    const filePath = path.join(DOWNLOAD_DIR, fileName);
    fs.writeFileSync(filePath, response.data);
    downloadedFiles.add(fileName);
    log(`✓ Descargado: ${fileName}`);
    return true;
  } catch (error) {
    log(`✗ Error descargando ${fileName}: ${error.message}`);
    failedFiles.push({ file: fileName, error: error.message });
    return false;
  }
}

function extractReferences(content, baseUrl) {
  const references = [];
  
  // Buscar xsl:include y xsl:import
  const includeRegex = /xsl:(include|import)\s+href=['"]([^'"]+)['"]/g;
  let match;
  
  while ((match = includeRegex.exec(content)) !== null) {
    const href = match[2];
    const fullUrl = new URL(href, baseUrl).href;
    const fileName = href.split('/').pop();
    
    references.push({
      url: fullUrl,
      fileName: fileName,
      type: match[1]
    });
  }
  
  return references;
}

async function processFile(fileUrl, fileName, baseUrl = BASE_URL, depth = 0) {
  const indent = '  '.repeat(depth);
  
  try {
    const response = await axios.get(fileUrl, {
      timeout: 10000
    });
    
    const content = response.data;
    
    // Descargar el archivo actual
    await downloadFile(fileUrl, fileName);
    
    // Extraer referencias
    const references = extractReferences(content, baseUrl);
    
    if (references.length > 0) {
      log(`${indent}→ Referencias en ${fileName}: ${references.length}`);
      
      // Descargar archivos referenciados recursivamente
      for (const ref of references) {
        const refUrl = ref.url;
        const refFileName = ref.fileName;
        
        log(`${indent}  - ${ref.type} ${refFileName}`);
        
        if (!downloadedFiles.has(refFileName)) {
          await new Promise(r => setTimeout(r, 300)); // Delay para no sobrecargar
          await processFile(refUrl, refFileName, baseUrl, depth + 1);
        }
      }
    }
    
    return true;
  } catch (error) {
    log(`✗ Error procesando ${fileName}: ${error.message}`);
    failedFiles.push({ file: fileName, error: error.message });
    return false;
  }
}

// Mapeo de URLs remotas a nombres de archivos locales
const urlMappings = {
  'http://www.sat.gob.mx/sitio_internet/cfd/2/cadenaoriginal_2_0/utilerias.xslt': 'utilerias.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/donat/donat11.xslt': 'donat11.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/divisas/divisas.xslt': 'divisas.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/implocal/implocal.xslt': 'implocal.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/leyendasFiscales/leyendasFisc.xslt': 'leyendasFisc.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/pfic/pfic.xslt': 'pfic.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/TuristaPasajeroExtranjero/TuristaPasajeroExtranjero.xslt': 'TuristaPasajeroExtranjero.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xslt': 'nomina12.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/cfdiregistrofiscal/cfdiregistrofiscal.xslt': 'cfdiregistrofiscal.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/pagoenespecie/pagoenespecie.xslt': 'pagoenespecie.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/aerolineas/aerolineas.xslt': 'aerolineas.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/valesdedespensa/valesdedespensa.xslt': 'valesdedespensa.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/notariospublicos/notariospublicos.xslt': 'notariospublicos.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/vehiculousado/vehiculousado.xslt': 'vehiculousado.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/servicioparcialconstruccion/servicioparcialconstruccion.xslt': 'servicioparcialconstruccion.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/renovacionysustitucionvehiculos/renovacionysustitucionvehiculos.xslt': 'renovacionysustitucionvehiculos.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/certificadodestruccion/certificadodedestruccion.xslt': 'certificadodedestruccion.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/arteantiguedades/obrasarteantiguedades.xslt': 'obrasarteantiguedades.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/ComercioExterior11/ComercioExterior11.xslt': 'ComercioExterior11.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/ComercioExterior20/ComercioExterior20.xslt': 'ComercioExterior20.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/ine/ine11.xslt': 'ine11.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/iedu/iedu.xslt': 'iedu.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/ventavehiculos/ventavehiculos11.xslt': 'ventavehiculos11.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/detallista/detallista.xslt': 'detallista.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/EstadoDeCuentaCombustible/ecc12.xslt': 'ecc12.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/consumodecombustibles/consumodeCombustibles11.xslt': 'consumodeCombustibles11.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/GastosHidrocarburos10/GastosHidrocarburos10.xslt': 'GastosHidrocarburos10.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/IngresosHidrocarburos10/IngresosHidrocarburos.xslt': 'IngresosHidrocarburos.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte20.xslt': 'CartaPorte20.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos20.xslt': 'Pagos20.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte30.xslt': 'CartaPorte30.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte31.xslt': 'CartaPorte31.xslt',
  'http://www.sat.gob.mx/sitio_internet/cfd/HidrocarburosPetro/hidrocarburospetroliferos.xslt': 'hidrocarburospetroliferos.xslt'
};

function convertReferencesToLocal() {
  log('\n' + '='.repeat(60));
  log('Convirtiendo referencias remotas a locales');
  log('='.repeat(60));

  try {
    const files = fs.readdirSync(DOWNLOAD_DIR)
      .filter(file => file.endsWith('.xslt'));

    let totalModifications = 0;

    files.forEach(file => {
      const filePath = path.join(DOWNLOAD_DIR, file);
      let content = fs.readFileSync(filePath, 'utf-8');
      let modifications = 0;

      // Reemplazar cada URL remota por su equivalente local
      for (const [remoteUrl, localFile] of Object.entries(urlMappings)) {
        const regex = new RegExp(`href="${remoteUrl}"`, 'g');
        const matches = content.match(regex) || [];
        
        if (matches.length > 0) {
          content = content.replace(regex, `href="${localFile}"`);
          modifications += matches.length;
        }
      }

      if (modifications > 0) {
        fs.writeFileSync(filePath, content, 'utf-8');
        log(`✓ ${file}: ${modifications} referencias actualizadas`);
        totalModifications += modifications;
      }
    });

    log(`\nTotal de referencias actualizadas: ${totalModifications}`);
    log('='.repeat(60));
  } catch (error) {
    log(`✗ Error en conversión: ${error.message}`);
  }
}

async function main() {
  log('='.repeat(60));
  log('Iniciando descarga de archivos XSLT del SAT');
  log('='.repeat(60));
  
  await ensureDir();
  
  const mainUrl = BASE_URL + MAIN_FILE;
  
  log(`\nArchivo principal: ${MAIN_FILE}`);
  log(`URL: ${mainUrl}`);
  log('');
  
  await processFile(mainUrl, MAIN_FILE);
  
  log('\n' + '='.repeat(60));
  log(`Total descargados: ${downloadedFiles.size}`);
  
  if (failedFiles.length > 0) {
    log(`\n⚠ Archivos no descargados (${failedFiles.length}):`);
    failedFiles.forEach(f => {
      log(`  - ${f.file}: ${f.error}`);
    });
  }
  
  // Convertir referencias a locales
  convertReferencesToLocal();
  
  log('Descarga completada');
}

main().catch(err => {
  log(`Error fatal: ${err.message}`);
  process.exit(1);
});
