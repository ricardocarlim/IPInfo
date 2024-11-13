using System.Collections.Generic;

public class LogradouroModel
{
    public List<LogradouroResource> Logradouros { get; set; }
}

public class LogradouroResource
{
    public int id { get; set; }    
    public string endereco { get; set; }   
    public int ClienteId { get; set; }
    
}
