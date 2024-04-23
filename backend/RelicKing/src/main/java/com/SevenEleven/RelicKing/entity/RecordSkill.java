package com.SevenEleven.RelicKing.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import jakarta.persistence.JoinColumn;
import jakarta.persistence.ManyToOne;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.ToString;

@Entity
@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@ToString
public class RecordSkill {

	@Id
	@GeneratedValue(strategy = GenerationType.IDENTITY)
	private int recordSkillId;

	@ManyToOne
	@JoinColumn(name = "record_id", referencedColumnName = "record_id", nullable = false)
	private Record record;

	@Column(nullable = false)
	private int skillNo;

	@Column(nullable = false)
	private int level;

	@Column(nullable = false)
	private int slot;

}
