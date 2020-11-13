using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    //ENDPOINT => URL 
    //https://localhost:5001 > c/ https
    //http://localhost:5000 > s/ https
    //https://localhost:5001/categories
    //Tasks trabalha de forma assincrona, não trava a thread principal
    //ActionResult, faz parte do base controller, retorna resultado do jeito que a tela espera..

    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet] //se não colocar nenhum verbo, será GET por default
        [Route("")]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)] //cache apenas para esse endpoint
        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)] se deixarmos a config de addCache do startup, ele vai cachear a aplicação inteira.. neste caso, podemos usar esta linha para dizer que tal ednpoint nao sera cacheado.
        public async Task<ActionResult<List<Category>>> Get([FromServices]DataContext context)
        { 
            var categories = await context.Categories.AsNoTracking().ToListAsync();//ao dar o ToListAsync ele executa a query, se precisar ordenar, filtar, ou qualoquer coisa.. fazer antes..
            return Ok(categories);
        }

        [HttpGet] 
        [Route("{id:int}")] //criamos restrição na rota, deste forma só vai permitir parâmetro inteiro.. *parametros recebidos pela URL são inseridos na Rota
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetById(int id, [FromServices]DataContext context){
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return Ok(category);
        }


        [HttpPost] //POST, PUT e DELETE os parametros são enviados via Body
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Post(
            [FromBody] Category model,//[FromBody] p/ indicar que vem do corpo do Json... ModelBinder vai ligar Json com o AspNet, converte json 
            [FromServices]DataContext context) //FromServices vem do serviço 
        {
            try
            {
                if(!ModelState.IsValid)//ModelState tem o estado do modelo passado... 
                return BadRequest(ModelState);  

                context.Categories.Add(model);//context, dbset, add e passa a categoria
                await context.SaveChangesAsync();//salva
                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new {message = "Não foi possivel cirar a categoria"});
            }
        }


        [HttpPut] 
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Put(int id, [FromBody]Category model, [FromServices]DataContext context) //envio pela rota e pelo corpo
        {
            //Verifica se o id informado é valido
            if(model.Id != id)
            return NotFound(new {message = "Categoria não encontrada"}); //criamos um objeto dinamico passando uma msg...
            //Verifica se os dados são válidos
            if (!ModelState.IsValid)
            return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified; //entrada de Category model. o EF vai verificar um a um oq foi alterado
                await context.SaveChangesAsync(); // só vai salvar oq foi alterado
                return Ok(model);
                
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new {message = "Não foi possivel atualizar a categoria"});
            }
        }

        [HttpDelete] 
        [Route("{id:int}")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<Category>> Delete(int id, [FromServices]DataContext context)
        {
            var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            return NotFound(new {message = "Categoria não encontrada"});

            try
            {
                context.Categories.Remove(category);
                await context.SaveChangesAsync();
                return Ok(new {message = "Categoria Removida"});
            }
            catch (Exception)
            {
               return BadRequest(new {message = "Não foi possivel excluir a categoria"});
            }
           
        }

        
    }
}
