const rabbit = require('../src/rabbit/index')


rabbit.getInstance()
  .then(broker => {
    broker.subscribe('Tracking.Relay.TrackingData', async (msg, ack) => {
      console.log('Message:', msg.content.toString())
      await SendTrackingData(JSON.parse(msg.content.toString()));
      ack()
    })
  })


async function SendTrackingData(message) {
  const broker = await rabbit.getInstance()
  await broker.send('Tracking.Agent.TrackingData', Buffer.from(JSON.stringify(message)))
}




