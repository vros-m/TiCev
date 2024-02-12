import React, { useState } from 'react';
import Avatar from '@mui/material/Avatar';
import Button from '@mui/material/Button';
import Tabs from '@mui/material/Tabs';
import Tab from '@mui/material/Tab';
import Typography from '@mui/material/Typography';
import Box from '@mui/material/Box';
import VideoCard from '../GeneralComponents/VideoCard'; 
import { Card,CardMedia,CardContent,Container,Grid, CardActionArea } from '@mui/material';
import { useLoaderData } from 'react-router-dom';
import { CURRENT_USER, parseAndFormatDate, profilePicture, thumbnailByVideoId, userController } from '../Constants';

const ProfilePage = () => {
  const [tabValue, setTabValue] = useState(0);

  const handleChangeTab = (event, newValue) => {
    setTabValue(newValue);
  };

  const  profile  = useLoaderData();
    return (
    
    <div style={{height:'calc(100vh - 110px)',marginTop:'110px'}} className='w-100 px-2 d-flex flex-column align-items-center'>
      <div style={{ display: 'flex', alignItems: 'center' }}>
        <Avatar src={profilePicture(profile.id)} alt="Profile Avatar" sx={{ width: 150, height: 150 }} />
              <div style={{ marginLeft: '20px' }} className='d-flex flex-column'>
            <Typography variant="h1">{profile.username}</Typography>
            <Typography variant="caption">{profile.username} • {profile.subscribers.toString()} {profile.subscribers == 1 ? 'subscriber' :
            'subscribers' }</Typography>
          <Button variant="outlined" style={{ marginTop: '10px' ,width:'200px'}}>
            Customize Profile
          </Button>
        </div>
      </div>

      {/* Tabs Section */}
      <Tabs value={tabValue} onChange={handleChangeTab} style={{ marginTop: '20px' }}>
        <Tab label="Home" />
          <Tab label="Playlists" />
          <Tab label="About" />
          {profile.feed && <Tab label="Feed" />}
      </Tabs>

      {/* Tab Content */}
      <TabPanel value={tabValue} index={0} >
                {/* Display Video Cards (Replace with your VideoCard component) */}
                
                <Container >
        <Grid container spacing={3}>
        {profile.videos.map((item, index) => {
          return <VideoCard props={item} key={index}/>
              })}
      </Grid></Container>
               
        {/* Add more VideoCard components as needed */}
      </TabPanel>

      <TabPanel value={tabValue} index={1}>
                {/* Display Playlist Cards (Replace with your PlaylistCard component) */}
                <Container >
        <Grid container spacing={3}>
        {profile.playlists.map((item, index) =>
                {
                    return <PlaylistCard key={index} info={item} />
                })}
      </Grid></Container>
              
        {/* Add more PlaylistCard components as needed */}
            </TabPanel>
            
            <TabPanel value={tabValue} index={2}>
                {/* User Information */}
                <div >
                <Typography variant="body1" paragraph style={{ textAlign: 'start' }}>
          Name: {profile.name}
        </Typography>
        <Typography variant="body1" paragraph style={{ textAlign: 'start' }}>
          Birthday: {parseAndFormatDate(profile.birthday)}
        </Typography>
        <Typography variant="body1" paragraph style={{ textAlign: 'start' }}>
          Gender: {profile.gender}
        </Typography>
        <Typography variant="body1" paragraph style={{ textAlign: 'start' }}>
          Email: {profile.email}
        </Typography>
        <Typography variant="body1" style={{ textAlign: 'start' }}>
          Bio: {profile.bio}
        </Typography>
                </div>
      
        </TabPanel>
        {profile.feed&&<TabPanel value={tabValue} index={3} >
          {/* Display Video Cards (Replace with your VideoCard component) */}
                
          <Container >
            <Grid container spacing={3}>
              {profile.feed.map((item, index) => {
                return <VideoCard props={item} key={index} />
              })}
            </Grid></Container>
               
          {/* Add more VideoCard components as needed */}
        </TabPanel>}
    </div>
  );
};

// Function to handle different content for each tab
const TabPanel = ({ children, value, index }) => (
  <div role="tabpanel" hidden={value !== index} style={{width:'100%'}}>
        {value === index && <Box style={{display:'flex',flexDirection:'column',alignItems:'center'}}>
        {children}</Box>}
  </div>
);

const PlaylistCard = ({ info }) => {
  const { title, videoIds } = info;
  const thumbnail = videoIds.length != 0 ? thumbnailByVideoId(videoIds[0]) : ''
  return (
        <Grid item xs={12} sm={6} md={4}>
            <Card sx={{
                maxWidth: 300, margin: '10px', transition: 'transform 0.2s ease-in-out',
                ':hover': {
                    transform: 'scale(1.05)',
                }
            }}>
            <CardActionArea>
        {/* Assuming 'thumbnail' is the path to the first video's thumbnail */}
        <CardMedia
          component="img"
          height="140"
          image={thumbnail}
          alt={title}
                />

                <CardContent>
          <Typography variant="subtitle1" fontWeight="bold">
            {title}
          </Typography>
        </CardContent>

        </CardActionArea>
            </Card>
            </Grid>
    );
  };

export async function ProfileLoader({ params })
{
  const currentUser = JSON.parse(localStorage.getItem(CURRENT_USER))
  const targetUser = params.userId!=undefined?params.userId:currentUser.id
  const profileResponse = await fetch(userController + `/GetUser/${targetUser}/${currentUser.id}`)
  if (profileResponse.ok)
  {
    const profile = await profileResponse.json()
    if (targetUser == currentUser.id)
    {
      const feedResponse = await fetch(userController + `/GetFeed/${currentUser.id}`)
      if (feedResponse.ok) {
        const feed = await feedResponse.json()
        profile.feed = feed
      }
      else
        profile.feed = null;
    }
    return profile
  }
  else throw new Error("Failed to fetch profile.")
    /* const videos = new Array(30).fill({})
    const playlists=new Array(30).fill({thumbnail:"https://via.placeholder.com/345x200",name:'NAME'})
    return {videos,playlists} */
}
export default ProfilePage;