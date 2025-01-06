import React from 'react';

export default class Instructions extends React.Component
{
    render() : React.JSX.Element
    {
        return <div> 
            <img src={`${process.env.PUBLIC_URL}/illustration.svg`} className="illustration" alt="illustration" />
            <p className="instructions">
                When you look at your huge game librar, and try to sift through hundreds of games looking for your next game to play, you have no idea how to decide.<br/><br/>
                Just upload your game library, and you will get back a '.csv' file with useful info for each game, like how long it will take to beat and its review score.<br/><br/>
                Import this '.csv' file into Excel, LibreOffice Calc, or Google Sheets, organize and sort the info however you like, and make it easier to decide what to play next.<br/><br/>
                Note: Game library file is a text file with the name of the games on newline, each line should not exceed 10 charcters and the full size of the file should not exceed
                10 MBs.
            </p>
        </div>;
    }
}
