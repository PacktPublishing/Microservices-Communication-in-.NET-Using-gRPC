const connection = new signalR.HubConnectionBuilder()
    .withUrl("/jobsHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ReceiveMessage", (message) => addMessage(message));

$('#btn-send-single').click(function () {
    var jobDescription = $('#job-description').val();

    connection.invoke("SendSingleJob", jobDescription).catch(err => console.error(err.toString()));
});

$('#btn-send-multiple').click(function () {
    var numberOfJobs = parseInt($('#jobs-to-send').val(), 10);
    var subject = new signalR.Subject();
    
    var iteration = 0;
    var intervalHandle = setInterval(() => {
        iteration++;
        subject.next(iteration);
        if (iteration === numberOfJobs) {
            clearInterval(intervalHandle);
            subject.complete();
        }
    }, 2000);

    connection.send("StreamJobs", subject);
});

$('#btn-trigger-multiple').click(function () {
    var numberOfJobs = parseInt($('#jobs-to-trigger').val(), 10);

    connection.stream("TriggerJobs", numberOfJobs)
        .subscribe({
            next: (message) => addMessage(message)
        });
});

async function start() {
    try {
        await connection.start();
        console.log('connected');
    } catch (err) {
        console.log(err);
        setTimeout(() => start(), 5000);
    }
};

connection.onclose(async () => {
    await start();
});

start();

function addMessage(message) {
    $('#signalr-message-panel').prepend($('<div />').text(message));
}