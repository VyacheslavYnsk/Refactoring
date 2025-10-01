public interface ISeatCategoryService
{
    Task<SeatCategory> CreateAsync(SeatCategoryCreate seatCategoryCreate);
    Task<PaginationResponse<SeatCategory>> GetListAsync(int page, int size); 

    Task<SeatCategory> GetByIdAsync(Guid id);

    Task<SeatCategory> EditAsync(SeatCategoryUpdate hallUpdate, Guid id);

    Task<bool> DeleteAsync(Guid id);


}