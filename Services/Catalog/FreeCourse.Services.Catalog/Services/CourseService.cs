using AutoMapper;
using Azure;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    internal class CourseService
    {
        private readonly IMongoCollection<Course> _coursecollection;
        private readonly IMongoCollection<Category> _categorycollection;
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper,IDatabaseSettings databaseSettings)
        {
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _coursecollection = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _categorycollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;

            _mapper = mapper;
        }


        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses =await _coursecollection.Find(course=>true).ToListAsync();

            if (courses.Any())
            {
                foreach (var course in courses)
                {
                    course.Category=await _categorycollection.Find<Category>(x=>x.Id==course.CategoryId).FirstAsync();
                }
            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses),200);

        }


    } 

}
