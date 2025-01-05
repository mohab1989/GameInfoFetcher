import React from 'react';
import Header from './components/Header.tsx'
import Instructions from './components/Instructions.tsx'
import UploadButton from './components/UploadButton.tsx';

export default class App extends React.Component
{
    render() : React.JSX.Element
    {
        return (
        <div className="landingpage">
            <Header/>
            <Instructions/>
            <UploadButton/>
        </div>);
    }
}
