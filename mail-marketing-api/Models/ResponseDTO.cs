using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    public class ResponseDTO<T>
    {
        public int Code { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = "";
        public bool IsSuccessed { get; set; } = false;
    }
}

