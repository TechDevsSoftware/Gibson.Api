using System;
using System.Threading.Tasks;
using TechDevs.Shared.Models;

namespace TechDevs.Clients.BookingRequests
{
    public interface IBookingRequestService
    {
        Task<BookingRequest> CreateBookingRequest(BookingRequest request, string clientId);
        Task<BookingRequest> UpdateBookingRequest(BookingRequest request, string clientId);
        Task DeleteBookingRequest(Guid id, string clientId);
        Task ConfirmBooking(Guid id, string clientId);
        Task CancelBooking(Guid id, string clientId);
    }

    public class BookingRequestService : IBookingRequestService
    {
        private readonly IClientRepository _clientRepository;

        public BookingRequestService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task CancelBooking(Guid id, string clientId)
        {
            var client = await _clientRepository.GetClient(clientId);
            var existingIndex = client.ClientData.BookingRequests.FindIndex(x => x.Id == id);
            if (existingIndex == -1) throw new Exception("Could not find existing booking request");
            client.ClientData.BookingRequests[existingIndex].Cancelled = true;
            // Send the email confirmation of cancellation
            var result = await _clientRepository.UpdateClient("ClientData.BookingRequests", client.ClientData.BookingRequests, clientId);
            return;
        }

        public async Task ConfirmBooking(Guid id, string clientId)
        {
            var client = await _clientRepository.GetClient(clientId);
            var existingIndex = client.ClientData.BookingRequests.FindIndex(x => x.Id == id);
            if (existingIndex == -1) throw new Exception("Could not find existing booking request");
            client.ClientData.BookingRequests[existingIndex].Confirmed = true;
            client.ClientData.BookingRequests[existingIndex].ConfirmationEmailSent = true;
            // Send the email confirmation
            var result = await _clientRepository.UpdateClient("ClientData.BookingRequests", client.ClientData.BookingRequests, clientId);
            return;
        }

        public async Task<BookingRequest> CreateBookingRequest(BookingRequest request, string clientId)
        {
            // Get the clinet
            var client = await _clientRepository.GetClient(clientId);
            client.ClientData.BookingRequests.Add(request);
            var result = await _clientRepository.UpdateClient("ClientData.BookingRequests", client.ClientData.BookingRequests, clientId);
            return request;
        }

        public async Task DeleteBookingRequest(Guid id, string clientId)
        {
            var client = await _clientRepository.GetClient(clientId);
            var existingIndex = client.ClientData.BookingRequests.FindIndex(x => x.Id == id);
            if (existingIndex == -1) throw new Exception("Could not find existing booking request");
            client.ClientData.BookingRequests.RemoveAt(existingIndex);
            var result = await _clientRepository.UpdateClient("ClientData.BookingRequests", client.ClientData.BookingRequests, clientId);
            return;
        }

        public async Task<BookingRequest> UpdateBookingRequest(BookingRequest request, string clientId)
        {
            var client = await _clientRepository.GetClient(clientId);
            var existingIndex = client.ClientData.BookingRequests.FindIndex(x => x.Id == request.Id);
            if (existingIndex == -1) throw new Exception("Could not find existing booking request");
            client.ClientData.BookingRequests[existingIndex] = request;
            var result = await _clientRepository.UpdateClient("ClientData.BookingRequests", client.ClientData.BookingRequests, clientId);
            return request;
        }
    }
}
