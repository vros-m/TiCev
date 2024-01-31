import React, { useState,useEffect } from 'react';
import {
  Container, IconButton, Typography, Paper, Button, Rating, Avatar, TextField, Card, CardContent, CardHeader, Divider,
  Box, IconButton as MuiIconButton,Grid,CardMedia, CardActionArea
} from '@mui/material';
import { Save, ThumbUpAlt } from '@mui/icons-material';
import {useLoaderData} from 'react-router-dom'

function VideoPlayer() {

  const [rating, setRating] = useState(0);
  const [isDescriptionOpen, setDescriptionOpen] = useState(false);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState('');

  const { recommendedVideos,playlist } = useLoaderData();

  function handleRatingChange(_, newValue) {
    setRating(newValue);
  }

  function handleDescriptionToggle() {
    setDescriptionOpen(!isDescriptionOpen);
  }

  function handleCommentSubmit() {
    if (newComment.trim() !== '') {
      setComments([...comments, { id: comments.length + 1, text: newComment }]);
      setNewComment('');
    }
  }

  return (
    <div className='d-flex flex-row justify-content-start w-100' style={{
      marginTop:'120px'
    }}>
      <Container sx={{marginRight:'0px'}} >    
            <Paper elevation={3} sx={{ padding: 2 }} height={'500px'} position={'relative'}>
              <Typography variant="h5" gutterBottom>
                Video Title
          </Typography>
          <div style={{ position: 'relative',overflow:'hidden'}}>
            <video style={{
              width: '100%', height:'500px'
          }}
                      controls={true}
            width="100%"
            src="https://dl164.filemate25.shop/?file=M3R4SUNiN3JsOHJ6WWQ3aTdPRFA4NW1rRVJIOG12WWdrZFJ4OFJrM0FPQUhpNGdyM3VHMklzVUVEYlVhM291bUZKVlgvVC9XWlo2R0lGdlBzSkVxUjB5UjlzSTE1SHFkMVpjdlROMWtWQk85eWNDdWhtVXoyeUh6TzRyck42RkNkVlZ4bTNKbDRINnNtS21FcVZ2MW95bnJvRWlNTVJZNTlENFRMZnpDOUl0QjJHeVdQcUMwaGNaWCszYk5zOXhiMy9haXR3bnp3TGxqNlpwbVNCbHdjY1E9"
                          >
                  </video>

          </div> 

        <Box display="flex" alignItems="center" marginTop={1}>
                    <Avatar sx={{ marginRight: 1 }} />
                    <Typography>Channel Name</Typography>
                    <Button variant="outlined" sx={{ marginLeft: '20px' }}>
                    Subscribe
                      </Button>
                  <Rating name="video-rating" value={rating} onChange={handleRatingChange}
                                  sx={{marginLeft:'auto'}} />
                <IconButton>
                  <Save />
                </IconButton>
                
              </Box>
               
            </Paper>

        <Paper elevation={3} sx={{ padding: 2, marginTop: 2 }}>
              <Box display="flex" alignItems="center" marginBottom={2}>
                  <Typography variant="h6" >
                        Description
                      </Typography>
                      <Typography variant="h7" marginLeft={'auto'} >
                        1.5k views
                    </Typography>
                    {/* Rating */}
                      <Rating value={rating} readOnly sx={{ marginLeft: '10px' }} />
                    <Typography sx={{ marginLeft: 1 }}>{rating.toFixed(1)}</Typography>
                </Box>
          <Typography>
            Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed ac mi ut velit
            consectetur pellentesque. Nullam vel urna id lacus commodo sagittis. Duis
            auctor...
          </Typography>
          <MuiIconButton onClick={handleDescriptionToggle}>
            <i className={`bi bi-chevron-${isDescriptionOpen ? 'up' : 'down'}`} />
          </MuiIconButton>
        </Paper>
        {/* Comments */}
        <Paper elevation={3} sx={{ padding: 2, marginTop: 2 }}>
          <Typography variant="h6" gutterBottom>
            Comments ({comments.length})
          </Typography>
          <TextField
            label="Add a public comment..."
            variant="outlined"
            fullWidth
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
          />
          <Button variant="contained" color="primary" onClick={handleCommentSubmit}>
            Comment
          </Button>
          {comments.map((comment) => (
            <CommentCard key={comment.id} text={comment.text} />
          ))}
        </Paper>
      </Container>
      <div className='d-flex flex-column' style={{marginRight:'auto',width:'350px'}}>
      {playlist&&<PlaylistCard playlist={playlist}/>}
      <Grid item md={6} sx={{marginTop:'32px'}}>
        <Typography variant="h5" gutterBottom>
          Recommended Videos
        </Typography>
        {/* Map through recommended videos and render RecommendedVideoCard for each */}
        {recommendedVideos.map((video, index) => (
          <RecommendedVideoCard key={index} video={video} />
        ))}
      </Grid>
      </div>
         
    </div>
  );
}

function CommentCard({ text }) {
  return (
    <Card sx={{ marginTop: 2 }}>
      <CardHeader
        avatar={<Avatar />}
        title="Username"
        subheader="2 hours ago"
        action={<IconButton><ThumbUpAlt /></IconButton>}
      />
      <CardContent>
        <Typography>{text}</Typography>
      </CardContent>
      <Divider />
    </Card>
  );
}

const RecommendedVideoCard = ({ video,selected}) => {
  return (
    <Card sx={{
      display: 'flex', width: '100%', marginBottom: '2px',
      boxShadow: 'none',filter:selected?'brightness(95%)':'none', cursor: 'pointer',

    }}>
      <CardActionArea sx={{
        display: 'flex', width: '100%', 
        boxShadow: 'none',  cursor: 'pointer',
      }}>
       {/* Thumbnail */}
       <CardMedia
        component="img"
        alt={video.title}
        height="100"
        sx={{
          maxWidth:"150px"
        }}
        image={video.thumbnailUrl}
      />
      {/* Content */}
      <CardContent sx={{ flex: 1,padding:'8px',paddingTop:'0px'}}>
        {/* Title */}
        <Typography variant="h6" component="div">
          {video.title}
        </Typography>
        {/* Poster and Views */}
        <Typography variant="body2" color="text.secondary" sx={{
          display: 'flex', alignItems: 'start',
          flexDirection: 'column'
        }}>
          <div style={{ marginRight: '8px' }}>{video.poster}</div>
          <div>{video.views}</div>
        </Typography>
      </CardContent>
      </CardActionArea>
   
    </Card>
  );
};


const PlaylistCard = ({ playlist }) => {
  const {playlistName, playlistOwner, currentVideoIndex, totalVideos, videos }=playlist
  return (
    <Card sx={{ maxHeight: '650px'}}>
      <CardContent sx={{display:'flex',flexDirection:'column',padding:'0px'}}>
        {/* Playlist Name */}
        <div className="px-3 my-2">
          <Typography variant="h5" gutterBottom>
            {playlistName}
          </Typography>
          {/* Playlist Owner */}
          <Typography variant="caption" color="textSecondary" gutterBottom>
            {`By ${playlistOwner}`}
          </Typography>
          {/* Video Index / Total Videos */}
          <Typography variant="caption" color="textSecondary">
            {`Video ${currentVideoIndex} / ${totalVideos}`}
          </Typography>
        </div>
       
        {/* Videos List */}
        <div style={{ overflowY: 'scroll',maxHeight:'500px'}}>
          {/* Map through playlist videos and render them */}
          {videos.map((video, index) => (
            <RecommendedVideoCard key={index} video={video} selected={index+1==currentVideoIndex}/>
          ))}
        </div>
      </CardContent>
    </Card>
  );
};

export default VideoPlayer;

export async function VideoLoader({params})
{
  const recommendedVideos = new Array(20).fill({
    title: 'Title', thumbnailUrl: "https://via.placeholder.com/345x200"
    , poster: 'someDude', views: '50k'
  })
  const returnValue={recommendedVideos}
  if (params.playlistId)
  {
    //fetchPlaylist
    const playlist = {
      playlistName: "name", playlistOwner: 'me', currentVideoIndex: 1, totalVideos: 20,
      videos:recommendedVideos
    }
    returnValue['playlist']=playlist
  }
  else
    returnValue['playlist']=null
    return returnValue
}


