
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import './Login/Login.css'
import { Link, useNavigate } from 'react-router-dom';
import { userController } from '../Constants';
import { useContext, useState } from 'react';
import UserContext from './Contexts/UserContext';

export default function CustomizeProfile() { 

    const navigate = useNavigate()
    const [profileState,setProfileState] = useState({
        username:"",profilePicture:""
    })
    const [userState,setUserState] = useContext(UserContext)

    async function updateProfile()
    {
        let updateUsernameResponse,updateProfilePictureResponse
        if (profileState.username != '')
        {
            updateUsernameResponse = fetch(userController + `/ChangeUsername/${userState.id}/${profileState.username}`,
                {
                method:'PUT'
                })
        }
        if (profileState.profilePicture != '')
        {
            updateProfilePictureResponse = fetch(userController + `/ChangeProfilePicture/${userState.id}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type":'application/json'
                    },
                    body:JSON.stringify(profileState.profilePicture)
            })
        }
        if (updateUsernameResponse && (!(await updateUsernameResponse).ok))
            alert("Error while updating username.")
        else if (updateProfilePictureResponse)
            setUserState(oldValue =>
            {
                return {
                    ...oldValue,
                    username:profileState.username
                }
        })
        if (updateProfilePictureResponse && (!(await updateProfilePictureResponse).ok))
            alert("Error while updating profile picture")
        navigate('/profile')
    }
    function onChange(event)
    {
        setProfileState((oldValue) =>
        {
            return {
                ...oldValue,
                [event.target.name]:event.target.value
            }

        })
    }

    function onFileUpload(event)
    {
        const file = event.target.files[0]
        if (file == null) return
        const fileReader = new FileReader()
        fileReader.readAsDataURL(file)
        fileReader.onload=(ev) =>
        {
            const size =new TextEncoder().encode(fileReader.result).length
            if (size >= 15 * 1024 * 1024)
            {
                alert("Image too large!")
                return
            }
            alert("Image uploaded.")
            setProfileState(oldValue => {
                return {
                    ...oldValue,
                    profilePicture: fileReader.result
                }
            })
        }
    }

    return <div className="login_container">
        <h2>Update info</h2>
        <TextField id="Username" label="Username" variant="outlined" sx={{
            marginBottom: "10px"
            
        }}
            value={profileState.username}
            name='username'
        onChange={(onChange)}/>
        <label htmlFor='input_file'>Add profile picture.</label> <input id='input_file' type='file' onChange={onFileUpload}
        accept=".jpg, .jpeg, .png" ></input>
        <Button variant="text" sx={{
            width:"80px"
        }} id='Button' onClick={(ev) =>
        {
            updateProfile()
        }}>OK</Button>
       
    </div>
}
