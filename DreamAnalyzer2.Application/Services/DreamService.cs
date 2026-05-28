using DreamAnalyzer2.Application.Builders;
using DreamAnalyzer2.Application.DTOs.Requests.Dreams;
using DreamAnalyzer2.Application.DTOs.Responses;
using DreamAnalyzer2.Application.Interfaces;
using DreamAnalyzer2.Application.Interfaces.Services;
using DreamAnalyzer2.Domain.Entities;
using DreamAnalyzer2.Domain.Interfaces;
using DreamAnalyzer2.Shared.Shared;

namespace DreamAnalyzer2.Application.Services
{
    public class DreamService : IDreamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDreamRepository _repository;

        public DreamService(IUnitOfWork unitOfWork, IDreamRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public async Task<DreamResponseDto> CreateDreamAsync(Guid userId, CreateDreamDto createDreamDto)
        {
            var dream = DreamBuilder.Create()
                .ForUser(userId)
                .WithTitle(createDreamDto.Title)
                .WithContent(createDreamDto.Content)
                .WithDate(createDreamDto.DreamDate)
                .Build();

            await _repository.AddAsync(dream);
            await _unitOfWork.SaveChangesAsync();
            return new DreamResponseDto
            {
                Title = dream.Title,
                Content = dream.Content,
                Id = dream.Id,
                CreatedAt = dream.CreatedAt,
                DreamDate = dream.DreamDate
            };
        }

        public async Task DeleteDreamAsync(Guid userId, Guid id, bool isAdmin)
        {
            var dream = await _repository.GetByIdAsync(id);
            if (dream == null)
                throw new NotFoundException("No such dream");
            if (!isAdmin && dream.UserId != userId)
                throw new ValidationException("You can delete only your dreams");

            await _repository.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<DreamResponseDto>> GetAllDreamsAsync()
        {
            var dreams = await _repository.GetAllAsync();
            var responses = new List<DreamResponseDto>();
            foreach (var dream in dreams)
                responses.Add(ToResponse(dream));
            return responses;
        }

        public async Task<List<DreamResponseDto>> GetAllDreamsByUserIdAsync(Guid userId)
        {
            var dreams = await _repository.GetByUserIdAsync(userId);
            var responses = new List<DreamResponseDto>();
            foreach (var dream in dreams)
                responses.Add(ToResponse(dream));
            return responses;
        }

        public async Task<DreamResponseDto> GetDreamByIdAsync(Guid userId, Guid id)
        {
            var dream = await _repository.GetByIdAsync(id);
            if (dream == null)
                throw new NotFoundException("No such dream");
            if (dream.UserId != userId)
                throw new ValidationException("You can only view your own dreams");

            return ToResponse(dream);
        }

        public async Task<DreamResponseDto> UpdateDreamAsync(Guid userId, Guid id, UpdateDreamDto updateDreamDto)
        {
            var dream = await _repository.GetByIdAsync(id);
            if (dream == null)
                throw new NotFoundException("No such dream");
            if (dream.UserId != userId)
                throw new ValidationException("You can update only your dreams");

            dream.Update(updateDreamDto.Title, updateDreamDto.Content, updateDreamDto.DreamDate);
            _repository.Update(dream);
            await _unitOfWork.SaveChangesAsync();
            return new DreamResponseDto
            {
                Id = dream.Id,
                Title = dream.Title,
                Content = dream.Content,
                CreatedAt = dream.CreatedAt,
                DreamDate = dream.DreamDate
            };
        }

        private DreamResponseDto ToResponse(Dream dream)
        {
            return new DreamResponseDto
            {
                Id = dream.Id,
                Title = dream.Title,
                Content = dream.Content,
                DreamDate = dream.DreamDate,
                CreatedAt = dream.CreatedAt,
                Mood = dream.Analysis != null ? new MoodDto
                {
                    Name = dream.Analysis.Mood.Name,
                    ColorHex = dream.Analysis.Mood.ColorHex,
                    Description = dream.Analysis.Mood.Description,
                } : null
            };
        }
    }
}
