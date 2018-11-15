using System;
using System.Security.Claims;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace API.Helpers
{
    public static class Extensions
    {
        private static string _ADMIN = "admin";
        public static void AddPagination(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages){
            
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
            var camelCaseFormatter = new JsonSerializerSettings();
            camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
        public static void AddApplicationError(this HttpResponse response, string message) {
            response.Headers.Add("Application-Error", message);
            //notify client browser about using custom headers
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
        public static int CalculateAge(this User user) => (DateTime.Now - user.DateOfBirth).Days / 365; 
        
      }
}