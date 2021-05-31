// eslint-disable-next-line no-unused-vars
import React, {useState} from 'react';
import { useHistory } from "react-router-dom";



function RegistrationPage(props) {

    const defaultImage = "/icons/default-photo.jpg";
    const postUser = '/api/users';
    const postAvatar = '/avatar'
    const defaultLoginExceptionMessage = 'Empty User Name';
    const defaultInputNameClass = 'input-text';

    const [labelPasswordClass, setPasswordClass] = useState('label-text');
    const [labelUserNameClass, setUserNameClass] = useState('label-text');
    const [passwordException, setPasswordException] = useState('wrong-input');
    const [loginException, setLoginException] = useState('wrong-input');
    const [needMovePassword, setNeedMovePassword] = useState(true);
    const [needMoveUser, setNeedMoveUser] = useState(true);
    const [preview, setPreview] = useState(defaultImage);
    const [file, setFile] = useState();
    const [userObj, setUserObj] = useState(new Object({login: '', password: ''}));
    const [loginExceptionMessage, setLoginExceptionMessage] = useState(defaultLoginExceptionMessage);
    const [nameInputClass, setNameInputClass] = useState(defaultInputNameClass);
    const history = useHistory();

    const handlePasswordChange = (event) => {
        const value = event.target.value;
        setUserObj({...userObj, password: event.target.value});
        setNeedMovePassword(value === '');
    };

    const handleUserChange = (event) => {
        const value = event.target.value;
        setUserObj({...userObj, login: event.target.value});
        setNeedMoveUser(value === '');
    };

    const handleImageChange = (event) => {
        if (event.target.files[0]['type'].split('/')[0] === 'image') {
            const reader = new FileReader();
            const file = event.target.files[0];
            setFile(file);
            reader.onload = () => {
                setPreview(reader.result);
            }
            reader.readAsDataURL(file);
        } else {
            alert("You need to download image")
        }
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        if (!Object.values(userObj).includes('')) {

            const created = await fetch(postUser, {
                method: 'POST',
                body: JSON.stringify(userObj),
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (created.status !== 201) {
                setLoginExceptionMessage('This name is taken');
                setLoginException('visible-wrong-input');
                setNameInputClass(`${defaultInputNameClass} bad-input`);
                return;
            }

            const id = await created.json();
            props.setCurrentUser(id);

            if (defaultImage !== preview) {
                const dataForResponse = new FormData();
                dataForResponse.append('uploads', file);
                const response = await fetch(`${postUser}/${id}${postAvatar}`, {
                    method: 'POST',
                    body: dataForResponse
                });
            }
            history.push('/messenger')
        } else {
            if (userObj.login === '') {
                setLoginException('visible-wrong-input');
                setUserNameClass('label-text bad-input')
            }
            if (userObj.password === '') {
                setPasswordException('visible-wrong-input');
                setPasswordClass('label-text bad-input');
            }
        }
    }


    return (
        <div className="container">
            <div className="form-wrapper">
                <form className="form" method="POST" onSubmit={handleSubmit}>
                    <h3 className="sign-in-text">Sign in</h3>
                    <div>
                        <input className="input-file" id="image" type="file" autoComplete="off"
                               onChange={handleImageChange}/>
                        <label htmlFor="image">
                            <img className="preview" src={preview} alt="preview"/>
                        </label>
                    </div>
                    <div>
                        <label className={labelUserNameClass} htmlFor="user-name">User Name</label>
                        <input className={nameInputClass} type="text" id="user-name" autoComplete="off"
                               onFocus={() => {
                                   setNameInputClass(defaultInputNameClass);
                                   setLoginExceptionMessage(defaultLoginExceptionMessage);
                                   setUserNameClass('label-text input-focused');
                                   setLoginException('wrong-input');
                               }}
                               onChange={handleUserChange}
                               onBlur={() => {
                                   if (needMoveUser) setUserNameClass('label-text')
                               }}/>
                        <p className={loginException}>{loginExceptionMessage}</p>
                    </div>
                    <div>
                        <label className={labelPasswordClass} htmlFor="password">Password</label>
                        <input className="input-text" type="text" id="password" autoComplete="off"
                               onFocus={() => {
                                   setPasswordClass('label-text input-focused')
                                   setPasswordException('wrong-input');
                               }}
                               onChange={handlePasswordChange}
                               onBlur={() => {
                                   if (needMovePassword) setPasswordClass('label-text');
                               }}/>
                        <p className={passwordException}>Empty password</p>
                    </div>
                    <input type="submit" className="submit-form" value="Submit"/>
                </form>
            </div>
        </div>
    );
}


export default RegistrationPage


