using api.application.Interfaces;
using api.domain.Entities;

namespace api.application.Tests;


    public sealed class TaskRepository : ITaskRepository
    {
        public List<TaskItem> Items { get; } = [];

        public Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            Items.Add(taskItem);
            return Task.FromResult(taskItem);
        }

        public Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            var item = Items.FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (item is null)
            {
                return Task.FromResult(false);
            }

            Items.Remove(item);
            return Task.FromResult(true);
        }

        public Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(Items.FirstOrDefault(x => x.Id == id && x.UserId == userId));
        }

        public Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyList<TaskItem>>(Items.Where(x => x.UserId == userId).ToList());
        }

        public Task<bool> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
        {
            var index = Items.FindIndex(x => x.Id == taskItem.Id && x.UserId == taskItem.UserId);
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            Items[index] = taskItem;
            return Task.FromResult(true);
        }
    }

