import React from 'react';

export default class Instructions extends React.Component
{
    render() : React.JSX.Element
    {
        return <div> 
            <img src={`${process.env.PUBLIC_URL}/illustration.svg`} className="illustration" alt="illustration" />
            <p className="instructions">When you look at your huge game library and try to sift through hundreds of games looking for your next game to play, you have no idea how to decide. 
            Just upload your game library, and you’ll get back a CSV file with useful info for each game, like how long it will take to beat and its review score.
        
            Import this CSV file into Excel, LibreOffice Calc, or Google Sheets, organize and sort the info however you like, and make it easier to decide what to play next</p>
        </div>;
    }
}
