import * as React from 'react';
import RegistrationPage from "./RegistrationPage";
import Messenger from "./Messenger";
import LogInPage from "./LogInPage"

import {
    BrowserRouter as Router,
    Switch,
    Route,
    Redirect,
} from "react-router-dom";
import {useState} from "react";



function App() {
    const [currentUser,setCurrentUser] = useState('');

    return (
        <Router>
            <div className="app-container">
                <Switch>
                    <Route path='/registration'><RegistrationPage setCurrentUser = {setCurrentUser}/></Route>
                    <Route path='/messenger'><Messenger userId = {currentUser}/></Route>
                    <Route path='/logIn'><LogInPage setCurrentUser = {setCurrentUser}/></Route>
                    <Redirect from='/' to='/logIn'/>
                </Switch>
            </div>
        </Router>
    )
}


export default App
