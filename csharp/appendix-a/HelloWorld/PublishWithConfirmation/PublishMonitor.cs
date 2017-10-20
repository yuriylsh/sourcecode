using System;
using System.Collections.Generic;
using RabbitMQ.Client.Events;

namespace PublishWithConfirmation
{
    internal class PublishMonitor
    {
        private readonly HashSet<ulong> _sentMessageIds = new HashSet<ulong>();
        private ulong _lastConfirmedId, _lastUnconfirmedId;
        private ulong _sendMessageCounter;
        

        public void OnBasicAck(object sender, BasicAckEventArgs e) => OnBrokerCommandArrival(
            e.DeliveryTag,
            e.Multiple,
            "Messages {0}-{1} successfully published.",
            "Message {0} was successfuly published.",
            ref _lastConfirmedId
        );

        public void OnBasicNack(object sender, BasicNackEventArgs e) => OnBrokerCommandArrival(
            e.DeliveryTag,
            e.Multiple,
            "Messages {0}-{1} failed to be published.",
            "Message {0} failed to be published.",
            ref _lastUnconfirmedId
        );

        private void OnBrokerCommandArrival(ulong messageId, bool multiple, string multipleFormat, string singleFormat, ref ulong lastId)
        {
            if (multiple)
            {
                var rangeStart = lastId > 0 ? lastId + 1 : 1;
                RemoveMessageIdRange(rangeStart, messageId);
                Console.WriteLine(multipleFormat, rangeStart, messageId);
            }
            else
            {
                Console.WriteLine(singleFormat, messageId);
                _sentMessageIds.Remove(messageId);
            }
            lastId = messageId;
        }

        private void RemoveMessageIdRange(ulong rangeStart, ulong rangeEnd)
        {
            
            for (ulong id = rangeStart; id <= rangeEnd ; id++)
            {
                _sentMessageIds.Remove(id);
            }
        }

        public void NewMessagePublished() => _sentMessageIds.Add(++_sendMessageCounter);
    }
}