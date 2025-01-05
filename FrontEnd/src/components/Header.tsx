import React from 'react';

export default class Header extends React.Component
{
    render() : React.JSX.Element
    {
        return <div> 
                <img src={`${process.env.PUBLIC_URL}/logo.png`} className="logo" alt="game-info-fetcher-high-resolution-logo-1" />
                <p className="slogan">
                What to play next, made easier.</p>
            </div>;
    }
}
