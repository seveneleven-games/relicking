package com.SevenEleven.RelicKing.dto.request;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@Builder
@AllArgsConstructor
@NoArgsConstructor
public class StageRequestDTO {

	private int eliteKill;

	// "eliteKill": 엘리트 몬스터 잡은 개수,
	// "normalKill": 일반 몬스터 잡은 개수,
	// "stage": 스테이지 종류 int로 보내줌,
	// "difficulty": 스테이지 난이도를 int로 보내줌,
	// "skillList": [
	// {
	// 	"slot": 장착 슬롯,
	// 	"skillNo": 스킬 고유 번호,
	// 	"level": 스킬 레벨
	// },
	// 	...
	// {
	// 	"slot": 장착 슬롯,
	// 	"skillNo": 스킬 고유 번호,
	// 	"level": 스킬 레벨
	// }
	// ]

}
