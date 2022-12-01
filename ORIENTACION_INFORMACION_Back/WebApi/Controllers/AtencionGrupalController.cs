using Aplicacion.Services;
using AutoMapper;
using Dominio.Mapper.AtencionesGrupales;
using Dominio.Models.AtencionesGrupales;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;

using WebApi.Responses;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AtencionGrupalController : ControllerBase
    {
        private readonly IGenericService<AtencionGrupal> _service;
        private readonly IGenericService<AtencionGrupalAnexo> _anexo;
        private readonly IMapper _mapper;
        public AtencionGrupalController(IGenericService<AtencionGrupal> service, IGenericService<AtencionGrupalAnexo> anexo, IMapper mapper)
        {
            this._service = service;
            this._mapper  = mapper;
            this._anexo   = anexo;
        }


        [HttpPost("PostAtencionGrupal")]
        public async Task<ActionResult> PostAtencionGrupal( [FromForm] AtencionGrupalDTO atenciongrupal, [FromForm] List<IFormFile> files)
        {
            var response = new { Titulo = "Bien echo!", Mensaje = "Datos cargados", Codigo = HttpStatusCode.OK};

            try
            { 
                AtencionGrupalAnexo atenciongrupalanexo = new AtencionGrupalAnexo();
          

                AtencionGrupal atenciongrupalDTO = _mapper.Map<AtencionGrupal>(atenciongrupal);
                atenciongrupalDTO.DtFechaRegistro = DateTime.Now;


                //bool grupalanexo = await _anexo.CreateAsync(atenciongrupalanexo);
                bool guardo = await _service.CreateAsync(atenciongrupalDTO);
              

                if (!guardo)
                {
                    response = new { Titulo = "Algo salio mal", Mensaje = "No se pudo guardar atención grupal", Codigo = HttpStatusCode.BadRequest };
                }
                var modelResponse = new ModelResponse<AtencionGrupalDTO>(response.Codigo, response.Titulo, response.Mensaje, atenciongrupal);
                return StatusCode((int)modelResponse.Codigo, modelResponse);


                string rutaInicial = Environment.CurrentDirectory;
                var nombreArchivo = files[0].FileName;
                var rutaArchivo = rutaInicial + "/Upload/" + nombreArchivo;
                var archivoArray = nombreArchivo.Split(".");
                var extencion = archivoArray[archivoArray.Length - 1];

                

                //byte[] tamañoArchivo = System.IO.File.ReadAllBytes(rutaArchivo);

                //atenciongrupalanexo.IBytes = tamañoArchivo;
                //atenciongrupalanexo.VcNombre = nombreArchivo;

                // FileInfo file = new FileInfo(rutaArchivo);

                //if (file.Length > 1048576)
                //{
                //    response = new { Titulo = "Algo salio mal", Mensaje = "El archio PDF no debe superar los 2 megabytes", Codigo = HttpStatusCode.BadRequest };
                //    return StatusCode((int)response.Codigo, response);
                //}

                if (files.Count == 0)
                {
                    response = new { Titulo = "Algo salio mal", Mensaje = "No se recibio el archivo ", Codigo = HttpStatusCode.BadRequest };
                }


                if (extencion != "pdf")
                {
                    response = new { Titulo = "Algo salio mal", Mensaje = "El archio no contiene el formato PDF", Codigo = HttpStatusCode.BadRequest };
                    return StatusCode((int)response.Codigo, response);
                }
                else
                {
                   
                    if (files.Count == 1)
                    {
                        System.IO.File.Delete(rutaArchivo);
                    }

                    using (var str = System.IO.File.Create(rutaArchivo))
                    {
                        str.Position = 0;
                        await files[0].CopyToAsync(str);
                    }
                }

            }
            catch (Exception)
            {
               response = new { Titulo = "Algo salio mal", Mensaje = "Error cargando el archivo PDf", Codigo = HttpStatusCode.BadRequest };
                return StatusCode((int)response.Codigo, response);
            }
          
        }

    }
}




    
