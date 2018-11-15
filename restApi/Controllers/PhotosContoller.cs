using API.Data;
using API.Helpers;
using API.Dtos;
using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Security.Claims;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System;
using API.Models;
using System.Linq;

namespace API.Controllers
{
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosContoller : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosContoller(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._repo = repo;
            this._mapper = mapper;
            this._cloudinaryConfig = cloudinaryConfig;
            this._cloudinary = new Cloudinary(new Account(this._cloudinaryConfig.Value.CloudName,
                this._cloudinaryConfig.Value.ApiKey, this._cloudinaryConfig.Value.ApiSecret));
        }
        [HttpGet("{id}", Name = "GetPhoto" )]
        public async Task<IActionResult> GetPhoto(int id) {

            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

            return Ok(photo);
        }


        [HttpPost]
        [ServiceFilter(typeof(UpdateEntityAccessor))]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoDto)
        {
            if (photoDto.File == null || photoDto.File.Length == 0)
                return BadRequest(); 
            
            var imgFromCloud = Task.Run(() => this.UploadImageToCloud(photoDto.File));
            var userFromRepo = await this._repo.GetUser(userId);
            
            var img = await imgFromCloud;

            if (img.PublicId == null) 
                return BadRequest("Could not add the photo");
            
            photoDto.Url = img.Uri.ToString();
            photoDto.PublicId = img.PublicId;

            var photo = _mapper.Map<Photo>(photoDto);
            if (!userFromRepo.Photos.Any(p => p.IsMain)) 
                photo.IsMain = true;
            
            userFromRepo.Photos.Add(photo);
            
            if (await _repo.SaveAll()){
                // photo ref get if from db entity when _repo.SaveAll successfully executes
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo); 
                //created 201 response with route to source in location header
                return CreatedAtRoute(nameof(this.GetPhoto), new {id = photo.Id}, photoToReturn);
            };
            return BadRequest("Could not add the photo");
        }
        
        [ServiceFilter(typeof(UpdateEntityAccessor))]
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id){
          
             var user = await _repo.GetUser(userId);

            //not optinal linq query due mapping to inner join users with photos, but much less code:D
            //bettert solution'll be select directly from photos
            user.Photos.SingleOrDefault(p => p.IsMain).IsMain = false;
            user.Photos.SingleOrDefault(p => p.Id == id).IsMain = true; 
            if (await _repo.SaveAll())
                return NoContent();
            return BadRequest("Could not set photo as main");
        }

        [ServiceFilter(typeof(UpdateEntityAccessor))]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId,int id) {
            var photo = await _repo.GetPhoto(id);
            
            if (photo.PublicId != null) {
                var deleteParams = new DeletionParams(photo.PublicId);  
            
                var result = await Task.Run(() => _cloudinary.Destroy(deleteParams)); 
            
                if (result.Result == "ok" )
                    _repo.Delete(photo);
            } else {
                _repo.Delete(photo);
            }
            if (await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Failed to delete the photo");
            ;
        }

        #region privates
        private ImageUploadResult UploadImageToCloud(IFormFile image) 
        {
            var uploadResult = new ImageUploadResult();    
            using (var stream = image.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(image.Name, stream),
                    Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                };
                return _cloudinary.Upload(uploadParams);
            }
        }
        #endregion
    }
}