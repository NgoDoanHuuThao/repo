using appbook.Data;
using appbook.Models.Domain;
using appbook.Models.DTO;
using appbook.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace appbook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublisherRepository _publisherRepository;

        // Đã sửa lỗi dính tên tham số
        public PublishersController(AppDbContext dbContext, IPublisherRepository publisherRepository)
        {
            _dbContext = dbContext;
            _publisherRepository = publisherRepository;
        }

        [HttpGet("get-all-publisher")]
        public IActionResult GetAllPublisher()
        {
            var allPublishers = _publisherRepository.GetAllPublishers();
            return Ok(allPublishers);
        }

        [HttpGet("get-publisher-by-id/{id}")]
        public IActionResult GetPublisherById(int id)
        {
            var publisherWithId = _publisherRepository.GetPublisherById(id);
            if (publisherWithId == null)
            {
                return NotFound();
            }
            return Ok(publisherWithId);
        }

        [HttpPost("add-publisher")] // Đã sửa lỗi dấu ngoặc và route
        public IActionResult AddPublisher([FromBody] AddPublisherRequestDTO addPublisherRequestDTO)
        {
            var publisherAdd = _publisherRepository.AddPublisher(addPublisherRequestDTO);
            return Ok(publisherAdd);
        }

        [HttpPut("update-publisher-by-id/{id}")]
        public IActionResult UpdatePublisherById(int id, [FromBody] PublisherNoIdDTO publisherDTO)
        {
            var publisherUpdate = _publisherRepository.UpdatePublisherById(id, publisherDTO);
            return Ok(publisherUpdate);
        }

        [HttpDelete("delete-publisher-by-id/{id}")]
        public IActionResult DeletePublisherById(int id)
        {
            var publisherDelete = _publisherRepository.DeletePublisherById(id);
            if (publisherDelete == null)
            {
                return NotFound();
            }
            return Ok(publisherDelete);
        }
    }
}


