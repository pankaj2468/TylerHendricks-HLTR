using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace TylerHendricks_Core.Models
{
    public class DataTableLoad
    {
        public DataTableLoad()
        {

        }
        public DataTableLoadViewModel LoadData(HttpContext httpContext)
        {
            DataTableLoadViewModel model = new DataTableLoadViewModel();
            if (httpContext != null)
            {
                model.Draw = httpContext.Request.Form["draw"].FirstOrDefault();

                // Skip number of Rows count  
                model.Start = httpContext.Request.Form["start"].FirstOrDefault() != null ? Convert.ToInt32(httpContext.Request.Form["start"].FirstOrDefault()) : 0;

                // Paging Length 10,20  
                model.Length = httpContext.Request.Form["length"].FirstOrDefault();

                // Sort Column Name  
                model.SortColumn = httpContext.Request.Form["columns[" + httpContext.Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();

                // Sort Column Direction (asc, desc)  
                model.SortColumnDirection = httpContext.Request.Form["order[0][dir]"].FirstOrDefault();

                // Search Value from (Search box)  
                model.SearchValue = httpContext.Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10, 20, 50,100)  
                model.PageSize = model.Length != null ? Convert.ToInt32(model.Length) : 0;

                model.Skip = model.Start != 0 ? Convert.ToInt32(model.Start) : 0;
            }
            return model;
        }
    }
}
