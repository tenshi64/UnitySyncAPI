window.onload = () => {
    const paragraph = document.createElement("p");
    document.body.append(paragraph);

    paragraph.innerHTML = `<b>Resetting...</b>`;

    const seconds = 30; //set how long the timer should run before refreshing the site
    var timer = 0;

    setInterval(() => {
        var lastCharacter = "";
        
        if(seconds-timer != 1)
        {
            lastCharacter = "s";
        }
        else
        {
            lastCharacter = "";
        }

        paragraph.innerHTML = `<b>Automatic server refresh in:</b> ${seconds-timer} second${lastCharacter}.`;
        timer++;

        if(timer-1 >= seconds)
        {
            window.location.href = "";
        }
    }, 1000);
};