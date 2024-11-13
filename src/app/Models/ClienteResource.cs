using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class ClienteModel
{
    public List<ClienteResource> Clientes { get; set; }
}

public class ClienteResource
{    
    public int id { get; set; }    
    public string nome { get; set; }    
    public string email { get; set; }
    public string logotipo { get; set; }        
    public IFormFile imagem { get; set; }
    public IEnumerable<LogradouroResource> logradouros { get; set; }
}
